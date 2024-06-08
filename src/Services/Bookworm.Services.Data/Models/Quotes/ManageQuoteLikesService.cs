namespace Bookworm.Services.Data.Models.Quotes
{
    using System;
    using System.Threading.Tasks;

    using Bookworm.Data.Common.Repositories;
    using Bookworm.Data.Models;
    using Bookworm.Services.Data.Contracts.Quotes;
    using Microsoft.EntityFrameworkCore;

    using static Bookworm.Common.Constants.ErrorMessagesConstants.QuoteErrorMessagesConstants;

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
            var quote = await this.quoteRepository.All()
                .FirstOrDefaultAsync(x => x.Id == quoteId) ??
                throw new InvalidOperationException(QuoteWrongIdError);

            if (quote.UserId == userId)
            {
                throw new InvalidOperationException("User cannot like or unlike his or her quotes!");
            }

            var quoteLike = await this.quoteLikesRepository
                .AllAsNoTracking()
                .FirstOrDefaultAsync(ql
                    => ql.QuoteId == quoteId && ql.UserId == userId);

            if (quoteLike == null)
            {
                quote.Likes = ++quote.Likes;
                this.quoteRepository.Update(quote);
                await this.quoteRepository.SaveChangesAsync();

                quoteLike = new QuoteLike { QuoteId = quoteId, UserId = userId };
                await this.quoteLikesRepository.AddAsync(quoteLike);
                await this.quoteLikesRepository.SaveChangesAsync();
            }

            return quote.Likes;
        }

        public async Task<int> UnlikeAsync(int quoteId, string userId)
        {
            var quote = await this.quoteRepository.All()
                .FirstOrDefaultAsync(x => x.Id == quoteId) ??
                throw new InvalidOperationException(QuoteWrongIdError);

            if (quote.UserId == userId)
            {
                throw new InvalidOperationException("User cannot like or unlike his or her quotes!");
            }

            var quoteLike = await this.quoteLikesRepository.All()
                .FirstOrDefaultAsync(ql => ql.QuoteId == quoteId && ql.UserId == userId);

            if (quoteLike != null)
            {
                quote.Likes = --quote.Likes;
                this.quoteRepository.Update(quote);
                await this.quoteRepository.SaveChangesAsync();

                this.quoteLikesRepository.Delete(quoteLike);
                await this.quoteLikesRepository.SaveChangesAsync();
            }

            return quote.Likes;
        }
    }
}
