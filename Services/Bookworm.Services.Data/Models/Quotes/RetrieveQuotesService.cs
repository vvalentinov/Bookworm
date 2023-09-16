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
            List<QuoteViewModel> quotes = await this.quoteRepository
                .AllAsNoTracking()
                .Where(x => x.IsApproved && x.IsDeleted == false)
                .To<QuoteViewModel>()
                .OrderByDescending(x => x.Id)
                .ToListAsync();

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

        public async Task<QuoteListingViewModel> GetAllApprovedQuotesAsync()
        {
            List<QuoteViewModel> quotes = await this.quoteRepository
              .AllAsNoTracking()
              .Where(x => x.IsApproved)
              .OrderBy(x => x.CreatedOn)
              .To<QuoteViewModel>()
              .ToListAsync();

            return new QuoteListingViewModel() { Quotes = quotes };
        }

        public async Task<QuoteListingViewModel> GetAllUnapprovedQuotesAsync()
        {
            List<QuoteViewModel> quotes = await this.quoteRepository
              .AllAsNoTracking()
              .Where(x => x.IsApproved == false)
              .OrderBy(x => x.CreatedOn)
              .To<QuoteViewModel>()
              .ToListAsync();

            return new QuoteListingViewModel() { Quotes = quotes };
        }

        public async Task<int> GetUnapprovedQuotesCountAsync()
        {
            int unapprovedQuotesCount = await this.quoteRepository
                .AllAsNoTracking()
                .Where(quote => quote.IsApproved == false)
                .CountAsync();

            return unapprovedQuotesCount;
        }

        public async Task<QuoteListingViewModel> GetAllDeletedQuotesAsync()
        {
            List<QuoteViewModel> quotes = await this.quoteRepository
                .AllAsNoTrackingWithDeleted()
                .Where(x => x.IsDeleted)
                .OrderBy(x => x.CreatedOn)
                .To<QuoteViewModel>()
                .ToListAsync();

            return new QuoteListingViewModel() { Quotes = quotes };
        }

        public async Task<QuoteViewModel> GetQuoteByIdAsync(int quoteId)
        {
            Quote quote = await this.quoteRepository
                .AllAsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == quoteId);

            return new QuoteViewModel()
            {
                Id = quoteId,
                Content = quote.Content,
                AuthorName = quote.AuthorName,
                BookTitle = quote.BookTitle,
                MovieTitle = quote.MovieTitle,
            };
        }

        public async Task<T> GetRandomQuoteAsync<T>()
        {
            return await this.quoteRepository
                        .AllAsNoTracking()
                        .Where(x => x.IsApproved)
                        .OrderBy(x => Guid.NewGuid())
                        .To<T>()
                        .FirstOrDefaultAsync();
        }

        public async Task<UserQuotesViewModel> GetUserQuotesAsync(string userId)
        {
            List<QuoteViewModel> quotes = await this.quoteRepository
                                          .AllAsNoTracking()
                                          .Where(x => x.UserId == userId)
                                          .OrderByDescending(x => x.CreatedOn)
                                          .To<QuoteViewModel>()
                                          .ToListAsync();

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

        public async Task<List<QuoteViewModel>> GetQuotesByTypeAsync(string userId, QuoteType type)
        {
            switch (type)
            {
                case QuoteType.ApprovedQuote:
                    return await this.GetApprovedQuotesAsync(userId);
                case QuoteType.UnapprovedQuote:
                    return await this.GetUnapprovedQuotesAsync(userId);
                case QuoteType.MovieQuote:
                    return await this.GetMovieQuotesAsync(userId);
                case QuoteType.BookQuote:
                    return await this.GetBookQuotesAsync(userId);
                case QuoteType.GeneralQuote:
                    return await this.GetGeneralQuotesAsync(userId);
                case QuoteType.LikedQuote:
                    return await this.GetLikedQuotesAsync();
                default: return null;
            }
        }

        private async Task<List<QuoteViewModel>> GetLikedQuotesAsync()
        {
            List<QuoteViewModel> likedQuotes = await this.quoteRepository.AllAsNoTracking()
                                       .Where(quote => this.quoteLikesRepository
                                            .AllAsNoTracking()
                                            .Any(x => x.QuoteId == quote.Id && x.Likes > 0))
                                       .OrderByDescending(quote => quote.CreatedOn)
                                       .To<QuoteViewModel>()
                                       .ToListAsync();

            return likedQuotes;
        }

        private async Task<List<QuoteViewModel>> GetGeneralQuotesAsync(string userId)
        {
            List<QuoteViewModel> generalQuotes = await this.quoteRepository.AllAsNoTracking()
                                       .Where(quote => (userId == null || quote.UserId == userId) &&
                                                        quote.MovieTitle == null &&
                                                        quote.BookTitle == null)
                                       .OrderByDescending(quote => quote.CreatedOn)
                                       .To<QuoteViewModel>()
                                       .ToListAsync();

            return generalQuotes;
        }

        private async Task<List<QuoteViewModel>> GetBookQuotesAsync(string userId)
        {
            List<QuoteViewModel> bookQuotes = await this.quoteRepository.AllAsNoTracking()
                                       .Where(quote => (userId == null || quote.UserId == userId) && quote.BookTitle != null)
                                       .OrderByDescending(quote => quote.CreatedOn)
                                       .To<QuoteViewModel>()
                                       .ToListAsync();

            return bookQuotes;
        }

        private async Task<List<QuoteViewModel>> GetApprovedQuotesAsync(string userId)
        {
            List<QuoteViewModel> approvedQuotes = await this.quoteRepository.AllAsNoTracking()
                                       .Where(quote => (userId == null || quote.UserId == userId) && quote.IsApproved)
                                       .OrderByDescending(quote => quote.CreatedOn)
                                       .To<QuoteViewModel>()
                                       .ToListAsync();

            return approvedQuotes;
        }

        private async Task<List<QuoteViewModel>> GetUnapprovedQuotesAsync(string userId)
        {
            List<QuoteViewModel> unapprovedQuotes = await this.quoteRepository.AllAsNoTracking()
                                       .Where(quote => (userId == null || quote.UserId == userId) && quote.IsApproved == false)
                                       .OrderByDescending(quote => quote.CreatedOn)
                                       .To<QuoteViewModel>()
                                       .ToListAsync();

            return unapprovedQuotes;
        }

        private async Task<List<QuoteViewModel>> GetMovieQuotesAsync(string userId)
        {
            List<QuoteViewModel> movieQuotes = await this.quoteRepository.AllAsNoTracking()
                                       .Where(quote => (userId == null || quote.UserId == userId) && quote.MovieTitle != null)
                                       .OrderByDescending(quote => quote.CreatedOn)
                                       .To<QuoteViewModel>()
                                       .ToListAsync();

            return movieQuotes;
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
                .FirstOrDefaultAsync(x => x.QuoteId == quoteId && x.UserId == userId);

            return quote != null;
        }
    }
}
