namespace Bookworm.Services.Data.Models.Quotes
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Bookworm.Common.Enums;
    using Bookworm.Data.Common.Repositories;
    using Bookworm.Data.Models;
    using Bookworm.Data.Models.Enums;
    using Bookworm.Services.Data.Contracts.Quotes;
    using Bookworm.Services.Mapping;
    using Bookworm.Web.ViewModels.Quotes;
    using Microsoft.EntityFrameworkCore;

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

        public async Task<QuoteListingViewModel> GetAllApprovedAsync(
            string userId = null,
            int? page = null)
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
                    RecordsCount = await this.quoteRepository.AllAsNoTracking().CountAsync(),
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
                .FirstOrDefaultAsync(x => x.Id == quoteId);

            return new QuoteViewModel
            {
                Id = quoteId,
                Content = quote.Content,
                AuthorName = quote.AuthorName,
                BookTitle = quote.BookTitle,
                MovieTitle = quote.MovieTitle,
            };
        }

        public async Task<T> GetRandomAsync<T>() =>
            await this.quoteRepository
                        .AllAsNoTracking()
                        .Where(x => x.IsApproved)
                        .OrderBy(x => Guid.NewGuid())
                        .To<T>()
                        .FirstOrDefaultAsync();

        public async Task<QuoteListingViewModel> GetAllByTypeAsync(
            string sortCriteria,
            string userId,
            string type,
            string content,
            int page)
        {
            bool isValidSortCriteria = Enum.TryParse(sortCriteria, out SortQuotesCriteria sortQuotesCriteria);
            if (isValidSortCriteria == false)
            {
                throw new InvalidOperationException("Invalid sort quote criteria!");
            }

            var quotesQuery = this.quoteRepository
                .AllAsNoTracking()
                .Where(q => q.IsApproved)
                .To<QuoteViewModel>();

            return await this.RetrieveQuotesFromQueryAsync(
                quotesQuery,
                type,
                sortQuotesCriteria,
                content,
                userId,
                page);
        }

        private async Task<List<QuoteViewModel>> RetrieveQuoteUserStatusAsync(
            List<QuoteViewModel> quotes,
            string userId)
        {
            foreach (QuoteViewModel quote in quotes)
            {
                quote.IsLikedByUser = await this.quoteLikesRepository
                                                .AllAsNoTracking()
                                                .AnyAsync(ql => ql.QuoteId == quote.Id && ql.UserId == userId);

                quote.IsUserQuoteCreator = quote.UserId == userId;
            }

            return quotes;
        }

        private async Task<QuoteListingViewModel> RetrieveQuotesFromQueryAsync(
            IQueryable<QuoteViewModel> quotesQuery,
            string type,
            SortQuotesCriteria sortQuotesCriteria,
            string content,
            string userId,
            int page)
        {
            if (type != null)
            {
                bool isValidQuoteType = Enum.TryParse(type, out ApiQuoteType quoteType);
                if (isValidQuoteType == false)
                {
                    throw new InvalidOperationException("Invalid quote type!");
                }

                if (quoteType == ApiQuoteType.LikedQuote)
                {
                    quotesQuery = quotesQuery.Where(q => q.Likes > 0);
                }
                else
                {
                    quotesQuery = quotesQuery.Where(q => q.Type == (QuoteType)quoteType);
                }
            }

            if (string.IsNullOrWhiteSpace(content) == false)
            {
                quotesQuery = quotesQuery.Where(q => q.Content.ToLower().Contains(content.ToLower()));
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

            var allFilteredQuotes = await quotesQuery.ToListAsync();

            quotesQuery = quotesQuery.Skip((page - 1) * QuotesPerPage).Take(QuotesPerPage);

            var quotes = await this.RetrieveQuoteUserStatusAsync(await quotesQuery.ToListAsync(), userId);

            return new QuoteListingViewModel
            {
                Quotes = quotes,
                ItemsPerPage = QuotesPerPage,
                PageNumber = page,
                RecordsCount = allFilteredQuotes.Count,
            };
        }
    }
}
