namespace Bookworm.Services.Data.Models.Quotes
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Bookworm.Data.Common.Repositories;
    using Bookworm.Data.Models;
    using Bookworm.Data.Models.Enums;
    using Bookworm.Services.Data.Contracts.Quotes;
    using Bookworm.Services.Mapping;
    using Bookworm.Web.ViewModels.Quotes;
    using Microsoft.EntityFrameworkCore;

    public class RetrieveUserQuotesService : IRetrieveUserQuotesService
    {
        private readonly IDeletableEntityRepository<Quote> quoteRepository;
        private readonly IDeletableEntityRepository<QuoteLike> quoteLikeRepository;

        public RetrieveUserQuotesService(
            IDeletableEntityRepository<Quote> quoteRepository,
            IDeletableEntityRepository<QuoteLike> quoteLikeRepository)
        {
            this.quoteRepository = quoteRepository;
            this.quoteLikeRepository = quoteLikeRepository;
        }

        public async Task<UserQuoteListingViewModel> GetAllUserQuotesAsync(string userId)
        {
            List<QuoteViewModel> userQuotes = await this.GetUserQuotesAsync(userId);

            int approvedQuotesCount = userQuotes.Count(x => x.IsApproved);
            int unapprovedQuotesCount = userQuotes.Count(x => !x.IsApproved);

            UserQuoteListingViewModel model = new UserQuoteListingViewModel()
            {
                Quotes = userQuotes,
                ApprovedQuotesCount = approvedQuotesCount,
                UnapprovedQuotesCount = unapprovedQuotesCount,
            };

            return model;
        }

        public async Task<List<T>> GetUserApprovedQuotesAsync<T>(string userId)
        {
            return await this.quoteRepository
                .AllAsNoTracking()
                .Where(x => x.UserId == userId && x.IsApproved)
                .To<T>()
                .ToListAsync();
        }

        public async Task<List<T>> GetUserLikedQuotesAsync<T>(string userId)
        {
            List<QuoteViewModel> userQuotes = await this.GetUserQuotesAsync(userId);

            List<int> ids = userQuotes.Select(x => x.Id).ToList();

            return await this.quoteLikeRepository
                .AllAsNoTracking()
                .Where(x => ids.Contains(x.QuoteId))
                .To<T>()
                .ToListAsync();
        }

        public async Task<List<T>> GetUserQuotesByTypeAsync<T>(string userId, QuoteType type)
        {
            return await this.quoteRepository
                .AllAsNoTracking()
                .Where(x => x.UserId == userId && x.Type == type)
                .To<T>()
                .ToListAsync();
        }

        public async Task<List<T>> GetUserUnapprovedQuotesAsync<T>(string userId)
        {
            return await this.quoteRepository
                .AllAsNoTracking()
                .Where(x => x.UserId == userId && x.IsApproved == false)
                .To<T>()
                .ToListAsync();
        }

        private async Task<List<QuoteViewModel>> GetUserQuotesAsync(string userId)
        {
            return await this.quoteRepository
                .AllAsNoTracking()
                .Where(x => x.UserId == userId)
                .To<QuoteViewModel>()
                .ToListAsync();
        }
    }
}
