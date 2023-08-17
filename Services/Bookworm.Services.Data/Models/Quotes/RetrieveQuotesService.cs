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
    using Bookworm.Web.ViewModels.Quotes;
    using Microsoft.EntityFrameworkCore;

    public class RetrieveQuotesService : IRetrieveQuotesService
    {
        private readonly IDeletableEntityRepository<Quote> quoteRepository;
        private readonly IRepository<QuoteLike> quoteLikesRepository;
        private readonly IDeletableEntityRepository<UserQuoteLike> usersQuotesLikesRepository;

        public RetrieveQuotesService(
            IDeletableEntityRepository<Quote> quoteRepository,
            IRepository<QuoteLike> quoteLikesRepository,
            IDeletableEntityRepository<UserQuoteLike> usersQuotesLikesRepository)
        {
            this.quoteRepository = quoteRepository;
            this.quoteLikesRepository = quoteLikesRepository;
            this.usersQuotesLikesRepository = usersQuotesLikesRepository;
        }

        public async Task<QuoteListingViewModel> GetAllQuotesAsync(string userId)
        {
            List<QuoteViewModel> quotes = this.quoteRepository
                .AllAsNoTracking()
                .Where(x => x.IsApproved && x.IsDeleted == false)
                .To<QuoteViewModel>()
                .OrderByDescending(x => x.Id)
                .ToList();

            foreach (var quote in quotes)
            {
                quote.Likes = await this.GetQuoteLikesAsync(quote.Id);
                quote.HasBeenLiked = userId != null && await this.CheckIfQuoteHasBeenLikedAsync(quote.Id, userId);
            }

            return new QuoteListingViewModel()
            {
                Quotes = quotes,
            };
        }

        public IEnumerable<T> GetAllUnapprovedQuotes<T>()
        {
            return this.quoteRepository
              .AllAsNoTracking()
              .Where(x => x.IsApproved == false && x.IsDeleted == false)
              .OrderBy(x => x.CreatedOn)
              .To<T>()
              .ToList();
        }

        public QuoteViewModel GetQuoteById(int quoteId)
        {
            Quote quote = this.quoteRepository.All().First(x => x.Id == quoteId);

            return new QuoteViewModel()
            {
                Content = quote.Content,
                AuthorName = quote.AuthorName,
                BookTitle = quote.BookTitle,
                Id = quoteId,
                MovieTitle = quote.MovieTitle,
            };
        }

        public T GetRandomQuote<T>()
        {
            return this.quoteRepository
                        .AllAsNoTracking()
                        .Where(x => x.IsApproved == true && x.IsDeleted == false)
                        .OrderBy(x => Guid.NewGuid())
                        .To<T>()
                        .FirstOrDefault();
        }

        public async Task<UserQuotesViewModel> GetUserQuotesAsync(string userId)
        {
            List<QuoteViewModel> quotes = this.quoteRepository.AllAsNoTracking()
                                          .Where(x => x.UserId == userId && x.IsDeleted == false)
                                          .OrderByDescending(x => x.CreatedOn)
                                          .To<QuoteViewModel>()
                                          .ToList();

            int approvedQuotesCount = quotes.Where(x => x.IsApproved).Count();
            int unapprovedQuotesCount = quotes.Where(x => x.IsApproved == false).Count();

            foreach (var quote in quotes)
            {
                quote.Likes = await this.GetQuoteLikesAsync(quote.Id);
            }

            return new UserQuotesViewModel()
            {
                Quotes = quotes,
                ApprovedQuotesCount = approvedQuotesCount,
                UnapprovedQuotesCount = unapprovedQuotesCount,
            };
        }

        public List<QuoteViewModel> GetQuotesByType(string userId, QuoteType type)
        {
            List<QuoteViewModel> quotes = this.quoteRepository.AllAsNoTracking()
                                       .Where(x => userId == null || x.UserId == userId)
                                       .OrderByDescending(x => x.CreatedOn)
                                       .To<QuoteViewModel>()
                                       .ToList();
            switch (type)
            {
                case QuoteType.ApprovedQuote:
                    return quotes.Where(q => q.IsApproved).ToList();
                case QuoteType.UnapprovedQuote:
                    return quotes.Where(q => q.IsApproved == false).ToList();
                case QuoteType.MovieQuote:
                    return quotes.Where(q => q.MovieTitle != null).ToList();
                case QuoteType.BookQuote:
                    return quotes.Where(q => q.BookTitle != null).ToList();
                case QuoteType.GeneralQuote:
                    return quotes.Where(q => q.MovieTitle == null && q.BookTitle == null).ToList();
                default: return quotes;
            }
        }

        private async Task<int> GetQuoteLikesAsync(int quoteId)
        {
            QuoteLike quote = await this.quoteLikesRepository
                .AllAsNoTracking()
                .FirstOrDefaultAsync(x => x.QuoteId == quoteId);

            return quote == null ? 0 : quote.Likes;
        }

        private async Task<bool> CheckIfQuoteHasBeenLikedAsync(int quoteId, string userId)
        {
            UserQuoteLike quote = await this.usersQuotesLikesRepository
                .AllAsNoTracking()
                .FirstOrDefaultAsync(x =>
                    x.QuoteId == quoteId &&
                    x.UserId == userId &&
                    x.IsDeleted == false);

            return quote != null;
        }
    }
}
