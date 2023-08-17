namespace Bookworm.Services.Data.Models.Quotes
{
    using System.Threading.Tasks;

    using Bookworm.Data.Common.Repositories;
    using Bookworm.Data.Models;
    using Bookworm.Services.Data.Contracts.Quotes;
    using Microsoft.EntityFrameworkCore;

    public class ManageQuoteLikesService : IManageQuoteLikesService
    {
        private readonly IRepository<QuoteLike> quoteLikesRepository;
        private readonly IDeletableEntityRepository<UserQuoteLike> usersQuotesLikesRepository;

        public ManageQuoteLikesService(
            IRepository<QuoteLike> quoteLikesRepository,
            IDeletableEntityRepository<UserQuoteLike> usersQuotesLikesRepository)
        {
            this.quoteLikesRepository = quoteLikesRepository;
            this.usersQuotesLikesRepository = usersQuotesLikesRepository;
        }

        public async Task<int> LikeQuoteAsync(int quoteId, string userId)
        {
            QuoteLike quote = await this.quoteLikesRepository
                .All()
                .FirstOrDefaultAsync(x => x.QuoteId == quoteId);

            if (quote == null)
            {
                quote = new QuoteLike()
                {
                    QuoteId = quoteId,
                    Likes = 1,
                };

                await this.quoteLikesRepository.AddAsync(quote);
                await this.quoteLikesRepository.SaveChangesAsync();
            }
            else
            {
                quote.Likes++;
            }

            UserQuoteLike userQuoteLike = await this.usersQuotesLikesRepository
                .AllWithDeleted()
                .FirstOrDefaultAsync(x => x.QuoteId == quoteId && x.UserId == userId);

            if (userQuoteLike == null)
            {
                userQuoteLike = new UserQuoteLike()
                {
                    UserId = userId,
                    QuoteId = quoteId,
                };

                await this.usersQuotesLikesRepository.AddAsync(userQuoteLike);
                await this.usersQuotesLikesRepository.SaveChangesAsync();
            }
            else
            {
                userQuoteLike.IsDeleted = false;
            }

            await this.quoteLikesRepository.SaveChangesAsync();

            return quote.Likes;
        }

        public async Task<int> DislikeQuoteAsync(
            int quoteId,
            string userId)
        {
            QuoteLike quoteLike = await this.quoteLikesRepository
                .All()
                .FirstOrDefaultAsync(x => x.QuoteId == quoteId);

            UserQuoteLike userQuoteLike = await this.usersQuotesLikesRepository
                .All()
                .FirstOrDefaultAsync(x => x.QuoteId == quoteId && x.UserId == userId);

            userQuoteLike.IsDeleted = true;

            if (quoteLike.Likes > 0)
            {
                quoteLike.Likes--;
            }

            await this.quoteLikesRepository.SaveChangesAsync();
            return quoteLike.Likes;
        }
    }
}
