namespace Bookworm.Services.Data.Models.Quotes
{
    using System.Threading.Tasks;

    using Bookworm.Data.Common.Repositories;
    using Bookworm.Data.Models;
    using Bookworm.Services.Data.Contracts.Quotes;
    using Microsoft.EntityFrameworkCore;

    public class ManageQuoteLikesService : IManageQuoteLikesService
    {
        private readonly IDeletableEntityRepository<QuoteLike> quoteLikesRepository;
        private readonly IDeletableEntityRepository<Quote> quoteRepository;

        public ManageQuoteLikesService(
            IDeletableEntityRepository<QuoteLike> quoteLikesRepository,
            IDeletableEntityRepository<Quote> quoteRepository)
        {
            this.quoteLikesRepository = quoteLikesRepository;
            this.quoteRepository = quoteRepository;
        }

        public async Task<int> LikeQuoteAsync(int quoteId, string userId)
        {
            Quote quote = await this.quoteRepository.All().FirstOrDefaultAsync(x => x.Id == quoteId);
            quote.Likes++;
            this.quoteRepository.Update(quote);
            await this.quoteRepository.SaveChangesAsync();

            QuoteLike quoteLike = await this.quoteLikesRepository
                .AllAsNoTrackingWithDeleted()
                .FirstOrDefaultAsync(ql => ql.QuoteId == quoteId && ql.UserId == userId);

            if (quoteLike != null)
            {
                quoteLike.IsDeleted = false;
                this.quoteLikesRepository.Update(quoteLike);
            }
            else
            {
                quoteLike = new QuoteLike()
                {
                    QuoteId = quoteId,
                    UserId = userId,
                };

                await this.quoteLikesRepository.AddAsync(quoteLike);
            }

            await this.quoteLikesRepository.SaveChangesAsync();

            return await this.GetQuoteLikesCountAsync(quoteId);
        }

        public async Task<int> UnlikeQuoteAsync(int quoteId, string userId)
        {
            Quote quote = await this.quoteRepository.All().FirstOrDefaultAsync(x => x.Id == quoteId);
            if (quote.Likes - 1 < 0)
            {
                quote.Likes = 0;
            }
            else
            {
                quote.Likes--;
            }

            this.quoteRepository.Update(quote);
            await this.quoteRepository.SaveChangesAsync();

            QuoteLike quoteLike = await this.quoteLikesRepository
                .AllAsNoTracking()
                .FirstOrDefaultAsync(ql => ql.QuoteId == quoteId && ql.UserId == userId);

            if (quoteLike != null)
            {
                quoteLike.IsDeleted = true;
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
