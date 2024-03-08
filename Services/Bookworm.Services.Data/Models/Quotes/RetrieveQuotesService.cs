namespace Bookworm.Services.Data.Models.Quotes
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Bookworm.Common.Enums;
    using Bookworm.Data.Common.Repositories;
    using Bookworm.Data.Models;
    using Bookworm.Data.Models.DTOs;
    using Bookworm.Services.Data.Contracts.Quotes;
    using Bookworm.Services.Mapping;
    using Bookworm.Web.ViewModels.Quotes;
    using Bookworm.Web.ViewModels.Quotes.QuoteInputModels;
    using Microsoft.EntityFrameworkCore;

    using static Bookworm.Common.Quotes.QuotesActionsNamesConstants;
    using static Bookworm.Common.Quotes.QuotesDataConstants;

    public class RetrieveQuotesService : IRetrieveQuotesService
    {
        private readonly IDeletableEntityRepository<Quote> quoteRepository;
        private readonly IRepository<QuoteLike> quoteLikesRepository;

        public RetrieveQuotesService(
            IDeletableEntityRepository<Quote> quoteRepository,
            IRepository<QuoteLike> quoteLikesRepository)
        {
            this.quoteRepository = quoteRepository;
            this.quoteLikesRepository = quoteLikesRepository;
        }

        public async Task<QuoteListingViewModel> GetAllApprovedAsync(string userId = null, int? page = null)
        {
            var quotesQuery = this.quoteRepository
                .AllAsNoTracking()
                .Where(x => x.IsApproved)
                .OrderByDescending(x => x.CreatedOn)
                .To<QuoteViewModel>();

            if (page.HasValue)
            {
                var quotes = await quotesQuery
                    .Skip((page.Value - 1) * QuotesPerPage)
                    .Take(QuotesPerPage)
                    .ToListAsync();

                return new QuoteListingViewModel
                {
                    Quotes = userId != null ? await this.RetrieveQuoteUserStatusAsync(quotes, userId) : quotes,
                    PageNumber = page ?? 1,
                    ItemsPerPage = QuotesPerPage,
                    RecordsCount = await this.quoteRepository.AllAsNoTracking().CountAsync(x => x.IsApproved),
                };
            }
            else
            {
                return new QuoteListingViewModel { Quotes = await quotesQuery.ToListAsync() };
            }
        }

        public async Task<QuoteListingViewModel> GetAllUnapprovedAsync()
        {
            var quotes = await this.quoteRepository
              .AllAsNoTracking()
              .Where(x => x.IsApproved == false)
              .OrderByDescending(x => x.CreatedOn)
              .To<QuoteViewModel>()
              .ToListAsync();

            return new QuoteListingViewModel { Quotes = quotes };
        }

        public async Task<int> GetUnapprovedCountAsync() =>
            await this.quoteRepository
                .AllAsNoTracking()
                .Where(quote => quote.IsApproved == false)
                .CountAsync();

        public async Task<QuoteListingViewModel> GetAllDeletedAsync()
        {
            var quotes = await this.quoteRepository
                .AllAsNoTrackingWithDeleted()
                .Where(x => x.IsDeleted)
                .OrderBy(x => x.CreatedOn)
                .To<QuoteViewModel>()
                .ToListAsync();

            return new QuoteListingViewModel { Quotes = quotes };
        }

        public async Task<QuoteViewModel> GetByIdAsync(int quoteId)
        {
            var quote = await this.quoteRepository
                .AllAsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == quoteId) ??
                 throw new InvalidOperationException("No quote with given id found!");

            return new QuoteViewModel
            {
                Id = quoteId,
                Content = quote.Content,
                AuthorName = quote.AuthorName,
                BookTitle = quote.BookTitle,
                MovieTitle = quote.MovieTitle,
            };
        }

        public async Task<QuoteViewModel> GetRandomAsync() =>
                await this.quoteRepository
                        .AllAsNoTracking()
                        .Where(x => x.IsApproved)
                        .OrderBy(x => Guid.NewGuid())
                        .To<QuoteViewModel>()
                        .FirstOrDefaultAsync();

        public async Task<QuoteListingViewModel> GetAllUserQuotesAsync(string userId, int page)
        {
            var quotes = await this.quoteRepository
                .AllAsNoTracking()
                .Where(x => x.UserId == userId)
                .OrderByDescending(x => x.CreatedOn)
                .Skip((page - 1) * QuotesPerPage)
                .Take(QuotesPerPage)
                .To<QuoteViewModel>()
                .ToListAsync();

            return new QuoteListingViewModel
            {
                Quotes = quotes,
                ItemsPerPage = QuotesPerPage,
                PageNumber = page,
                RecordsCount = await this.quoteRepository.AllAsNoTracking().CountAsync(x => x.UserId == userId),
            };
        }

        public async Task<QuoteListingViewModel> GetAllByCriteriaAsync(GetQuotesApiDto getQuotesApiDto, string userId)
        {
            bool isValidSortCriteria = Enum.TryParse(getQuotesApiDto.SortCriteria, out SortQuotesCriteria sortQuotesCriteria);
            if (isValidSortCriteria == false)
            {
                throw new InvalidOperationException("Invalid sort quote criteria!");
            }

            var quotesQuery = this.quoteRepository.AllAsNoTracking().To<QuoteViewModel>();

            if (getQuotesApiDto.IsForUserQuotes)
            {
                quotesQuery = quotesQuery.Where(q => q.UserId == userId);
            }
            else
            {
                quotesQuery = quotesQuery.Where(q => q.IsApproved);
            }

            if (getQuotesApiDto.IsForUserQuotes && getQuotesApiDto.QuoteStatus != null)
            {
                bool isValidStatus = Enum.TryParse(getQuotesApiDto.QuoteStatus, out ApiQuoteStatus apiQuoteStatus);
                if (isValidStatus == false)
                {
                    throw new InvalidOperationException("Invalid quote status!");
                }

                switch (apiQuoteStatus)
                {
                    case ApiQuoteStatus.Approved:
                        quotesQuery = quotesQuery.Where(q => q.IsApproved);
                        break;
                    case ApiQuoteStatus.Unapproved:
                        quotesQuery = quotesQuery.Where(q => q.IsApproved == false);
                        break;
                }
            }

            bool isValidQuoteType = Enum.TryParse(getQuotesApiDto.Type, out ApiQuoteType quoteType);

            if (getQuotesApiDto.Type != null)
            {
                if (isValidQuoteType == false)
                {
                    throw new InvalidOperationException("Invalid quote type!");
                }

                switch (quoteType)
                {
                    case ApiQuoteType.BookQuote:
                        quotesQuery = quotesQuery.Where(q => q.Type == QuoteType.BookQuote);
                        break;
                    case ApiQuoteType.MovieQuote:
                        quotesQuery = quotesQuery.Where(q => q.Type == QuoteType.MovieQuote);
                        break;
                    case ApiQuoteType.GeneralQuote:
                        quotesQuery = quotesQuery.Where(q => q.Type == QuoteType.GeneralQuote);
                        break;
                    case ApiQuoteType.LikedQuote:
                        quotesQuery = quotesQuery.Where(q => q.Likes > 0);
                        break;
                }
            }

            if (string.IsNullOrWhiteSpace(getQuotesApiDto.Content) == false)
            {
                string content = getQuotesApiDto.Content.Trim().ToLower();
                switch (quoteType)
                {
                    case ApiQuoteType.BookQuote:
                        quotesQuery = quotesQuery.Where(q =>
                            q.Content.ToLower().Contains(content) ||
                            q.BookTitle.ToLower().Contains(content) ||
                            q.AuthorName.ToLower().Contains(content));
                        break;
                    case ApiQuoteType.MovieQuote:
                        quotesQuery = quotesQuery.Where(q =>
                            q.Content.ToLower().Contains(content) ||
                            q.MovieTitle.ToLower().Contains(content));
                        break;
                    case ApiQuoteType.GeneralQuote:
                        quotesQuery = quotesQuery.Where(q =>
                            q.Content.ToLower().Contains(content) ||
                            q.AuthorName.ToLower().Contains(content));
                        break;
                    default:
                        quotesQuery = quotesQuery.Where(q =>
                            q.Content.ToLower().Contains(content) ||
                            q.AuthorName.ToLower().Contains(content) ||
                            q.MovieTitle.ToLower().Contains(content) ||
                            q.BookTitle.ToLower().Contains(content));
                        break;
                }
            }

            switch (sortQuotesCriteria)
            {
                case SortQuotesCriteria.NewestToOldest:
                    quotesQuery = quotesQuery.OrderByDescending(q => q.CreatedOn);
                    break;
                case SortQuotesCriteria.OldestToNewest:
                    quotesQuery = quotesQuery.OrderBy(q => q.CreatedOn);
                    break;
                case SortQuotesCriteria.LikesCountDesc:
                    quotesQuery = quotesQuery.OrderByDescending(q => q.Likes);
                    break;
            }

            var totalCount = await quotesQuery.CountAsync();

            quotesQuery = quotesQuery.Skip((getQuotesApiDto.Page - 1) * QuotesPerPage).Take(QuotesPerPage);

            var quotes = await this.RetrieveQuoteUserStatusAsync(await quotesQuery.ToListAsync(), userId);

            return new QuoteListingViewModel
            {
                Quotes = quotes,
                ItemsPerPage = QuotesPerPage,
                PageNumber = getQuotesApiDto.Page,
                RecordsCount = totalCount,
            };
        }

        public async Task<(BaseQuoteInputModel, string)> GetQuoteForEditAsync(int id, string userId)
        {
            var quote = await this.quoteRepository.AllAsNoTracking().FirstOrDefaultAsync(q => q.Id == id) ??
                throw new InvalidOperationException("No quote with given id found!");

            if (quote.UserId != userId)
            {
                throw new InvalidOperationException("You have to be the quote's creator to edit it!");
            }

            switch (quote.Type)
            {
                case QuoteType.BookQuote:
                    return (AutoMapperConfig.MapperInstance.Map<BookQuoteInputModel>(quote), EditBookQuoteAction);
                case QuoteType.MovieQuote:
                    return (AutoMapperConfig.MapperInstance.Map<MovieQuoteInputModel>(quote), EditMovieQuoteAction);
                case QuoteType.GeneralQuote:
                    return (AutoMapperConfig.MapperInstance.Map<GeneralQuoteInputModel>(quote), EditGeneralQuoteAction);
                default: throw new InvalidOperationException("Invalid quote type!");
            }
        }

        private async Task<List<QuoteViewModel>> RetrieveQuoteUserStatusAsync(
            List<QuoteViewModel> quotes,
            string userId)
        {
            foreach (var quote in quotes)
            {
                quote.IsLikedByUser = await this.quoteLikesRepository
                                                .AllAsNoTracking()
                                                .AnyAsync(ql => ql.QuoteId == quote.Id && ql.UserId == userId);

                quote.IsUserQuoteCreator = quote.UserId == userId;
            }

            return quotes;
        }
    }
}
