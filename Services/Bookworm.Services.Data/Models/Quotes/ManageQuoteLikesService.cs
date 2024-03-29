﻿namespace Bookworm.Services.Data.Models.Quotes
{
    using System.Threading.Tasks;

    using Bookworm.Data.Common.Repositories;
    using Bookworm.Data.Models;
    using Bookworm.Services.Data.Contracts.Quotes;
    using Microsoft.EntityFrameworkCore;

    public class ManageQuoteLikesService : IManageQuoteLikesService
    {
        private readonly IRepository<QuoteLike> quoteLikesRepository;
        private readonly IDeletableEntityRepository<Quote> quoteRepository;

        public ManageQuoteLikesService(
            IRepository<QuoteLike> quoteLikesRepository,
            IDeletableEntityRepository<Quote> quoteRepository)
        {
            this.quoteLikesRepository = quoteLikesRepository;
            this.quoteRepository = quoteRepository;
        }

        public async Task<int> LikeAsync(int quoteId, string userId)
        {
            var quoteLike = await this.quoteLikesRepository
                .AllAsNoTracking()
                .FirstOrDefaultAsync(ql => ql.QuoteId == quoteId && ql.UserId == userId);

            if (quoteLike == null)
            {
                quoteLike = new QuoteLike { QuoteId = quoteId, UserId = userId };
                await this.quoteLikesRepository.AddAsync(quoteLike);
                await this.quoteLikesRepository.SaveChangesAsync();
            }

            return await this.GetQuoteLikesCountAsync(quoteId);
        }

        public async Task<int> UnlikeAsync(int quoteId, string userId)
        {
            QuoteLike quoteLike = await this.quoteLikesRepository
                .AllAsNoTracking()
                .FirstOrDefaultAsync(ql => ql.QuoteId == quoteId && ql.UserId == userId);

            if (quoteLike != null)
            {
                this.quoteLikesRepository.Update(quoteLike);
                await this.quoteLikesRepository.SaveChangesAsync();
            }

            return await this.GetQuoteLikesCountAsync(quoteId);
        }

        private async Task<int> GetQuoteLikesCountAsync(int quoteId)
        {
            return await this.quoteLikesRepository
                .AllAsNoTracking()
                .CountAsync(x => x.QuoteId == quoteId);
        }
    }
}
