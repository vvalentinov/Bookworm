namespace Bookworm.Services.Data.Models.Quotes
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Bookworm.Common.Enums;
    using Bookworm.Data.Common.Repositories;
    using Bookworm.Data.Models;
    using Bookworm.Services.Data.Contracts.Quotes;
    using Bookworm.Services.Mapping;
    using Bookworm.Web.ViewModels.DTOs;
    using Bookworm.Web.ViewModels.Quotes;
    using Microsoft.EntityFrameworkCore;

    using static Bookworm.Common.Constants.DataConstants.QuoteDataConstants;
    using static Bookworm.Common.Constants.ErrorMessagesConstants.QuoteErrorMessagesConstants;
    using static Bookworm.Common.Enums.ApiQuoteStatus;
    using static Bookworm.Common.Enums.SortQuotesCriteria;

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

        public async Task<QuoteListingViewModel> GetAllApprovedAsync(int? page = null, string userId = null)
        {
            var quotesQuery = this.quoteRepository
                .AllAsNoTracking()
                .Where(x => x.IsApproved)
                .OrderByDescending(x => x.CreatedOn)
                .To<QuoteViewModel>();

            if (page.HasValue)
            {
                quotesQuery = quotesQuery
                    .Skip((page.Value - 1) * QuotesPerPage)
                    .Take(QuotesPerPage);
            }

            var quotes = await quotesQuery.ToListAsync();

            var model = new QuoteListingViewModel
            {
                PageNumber = page ?? 1,
                ItemsPerPage = QuotesPerPage,
                RecordsCount = await this.quoteRepository.AllAsNoTracking().CountAsync(x => x.IsApproved),
                Quotes = userId != null ? await this.RetrieveQuoteUserStatusAsync(quotes, userId) : quotes,
            };

            return model;
        }

        public async Task<QuoteListingViewModel> GetAllUnapprovedAsync()
            => new QuoteListingViewModel
               {
                   Quotes = await this.quoteRepository
                            .AllAsNoTracking()
                            .Where(x => x.IsApproved == false)
                            .OrderByDescending(x => x.CreatedOn)
                            .To<QuoteViewModel>()
                            .ToListAsync(),
               };

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
                 throw new InvalidOperationException(QuoteWrongIdError);

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
                RecordsCount = await this.quoteRepository
                    .AllAsNoTracking()
                    .CountAsync(x => x.UserId == userId),
            };
        }

        public async Task<UploadQuoteViewModel> GetQuoteForEditAsync(int quoteId, string userId)
        {
            var quote = await this.quoteRepository
                .AllAsNoTracking()
                .FirstOrDefaultAsync(q => q.Id == quoteId) ??
                throw new InvalidOperationException(QuoteWrongIdError);

            if (quote.UserId != userId)
            {
                throw new InvalidOperationException(QuoteEditError);
            }

            return AutoMapperConfig.MapperInstance.Map<UploadQuoteViewModel>(quote);
        }

        public async Task<QuoteListingViewModel> GetAllByCriteriaAsync(string userId, GetQuotesApiDto getQuotesApiDto)
        {
            bool isValidSortCriteria = Enum.TryParse(
                getQuotesApiDto.SortCriteria,
                out SortQuotesCriteria sortQuotesCriteria);

            if (isValidSortCriteria == false)
            {
                throw new InvalidOperationException("Invalid sort quote criteria!");
            }

            var quotesQuery = this.quoteRepository.AllAsNoTracking().To<QuoteViewModel>();

            quotesQuery = getQuotesApiDto.IsForUserQuotes ?
                quotesQuery.Where(q => q.UserId == userId) :
                quotesQuery.Where(q => q.IsApproved);

            if (getQuotesApiDto.IsForUserQuotes && !string.IsNullOrWhiteSpace(getQuotesApiDto.QuoteStatus))
            {
                bool isValidStatus = Enum.TryParse(
                    getQuotesApiDto.QuoteStatus,
                    out ApiQuoteStatus apiQuoteStatus);

                if (isValidStatus == false)
                {
                    throw new InvalidOperationException("Invalid quote status!");
                }

                quotesQuery = quotesQuery.Where(q => apiQuoteStatus == Approved ? q.IsApproved : !q.IsApproved);
            }

            bool isValidQuoteType = Enum.TryParse(getQuotesApiDto.Type, out ApiQuoteType quoteType);

            if (!string.IsNullOrWhiteSpace(getQuotesApiDto.Type))
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

            if (!string.IsNullOrWhiteSpace(getQuotesApiDto.Content))
            {
                string content = getQuotesApiDto.Content.Trim();

                quotesQuery = quoteType switch
                {
                    ApiQuoteType.BookQuote => quotesQuery.Where(q =>
                                                EF.Functions.Like(q.Content, $"%{content}%") ||
                                                EF.Functions.Like(q.BookTitle, $"%{content}%") ||
                                                EF.Functions.Like(q.AuthorName, $"%{content}%")),
                    ApiQuoteType.MovieQuote => quotesQuery.Where(q =>
                                                EF.Functions.Like(q.Content, $"%{content}%") ||
                                                EF.Functions.Like(q.MovieTitle, $"%{content}%")),
                    ApiQuoteType.GeneralQuote => quotesQuery.Where(q =>
                                                EF.Functions.Like(q.Content, $"%{content}%") ||
                                                EF.Functions.Like(q.AuthorName, $"%{content}%")),
                    _ => quotesQuery.Where(q => EF.Functions.Like(q.Content, $"%{content}%") ||
                                                EF.Functions.Like(q.BookTitle, $"%{content}%") ||
                                                EF.Functions.Like(q.AuthorName, $"%{content}%") ||
                                                EF.Functions.Like(q.MovieTitle, $"%{content}%")),
                };
            }

            switch (sortQuotesCriteria)
            {
                case OldestToNewest:
                    quotesQuery = quotesQuery.OrderBy(q => q.CreatedOn);
                    break;
                case NewestToOldest:
                    quotesQuery = quotesQuery.OrderByDescending(q => q.CreatedOn);
                    break;
                case LikesCountDesc:
                    quotesQuery = quotesQuery.OrderByDescending(q => q.Likes);
                    break;
            }

            int recordsCount = await quotesQuery.CountAsync();

            quotesQuery = quotesQuery
                .Skip((getQuotesApiDto.Page - 1) * QuotesPerPage)
                .Take(QuotesPerPage);

            var quotes = await quotesQuery.ToListAsync();

            return new QuoteListingViewModel
            {
                Quotes = userId != null ? await this.RetrieveQuoteUserStatusAsync(quotes, userId) : quotes,
                ItemsPerPage = QuotesPerPage,
                PageNumber = getQuotesApiDto.Page,
                RecordsCount = recordsCount,
            };
        }

        private async Task<List<QuoteViewModel>> RetrieveQuoteUserStatusAsync(List<QuoteViewModel> quotes, string userId)
        {
            foreach (var quote in quotes)
            {
                quote.IsUserQuoteCreator = quote.UserId == userId;

                quote.IsLikedByUser = await this.quoteLikesRepository
                                                .AllAsNoTracking()
                                                .AnyAsync(ql => ql.QuoteId == quote.Id && ql.UserId == userId);
            }

            return quotes;
        }
    }
}
