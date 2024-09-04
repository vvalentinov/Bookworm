namespace Bookworm.Services.Data.Models.Quotes
{
    using System.Threading.Tasks;

    using Bookworm.Common;
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
            this.quoteRepository = quoteRepository;
            this.quoteLikesRepository = quoteLikesRepository;
        }

        public async Task<OperationResult<int>> LikeAsync(
            int quoteId,
            string userId)
        {
            var quote = await this.quoteRepository
                .All()
                .FirstOrDefaultAsync(x => x.Id == quoteId);

            if (quote == null)
            {
                return OperationResult.Fail<int>(QuoteWrongIdError);
            }

            if (quote.UserId == userId)
            {
                return OperationResult.Fail<int>("User cannot like or unlike his or her quotes!");
            }

            var quoteLike = await this.quoteLikesRepository
                .AllAsNoTracking()
                .FirstOrDefaultAsync(ql => ql.QuoteId == quoteId && ql.UserId == userId);

            if (quoteLike == null)
            {
                quote.Likes = ++quote.Likes;
                this.quoteRepository.Update(quote);
                await this.quoteRepository.SaveChangesAsync();

                quoteLike = new QuoteLike
                {
                    QuoteId = quoteId,
                    UserId = userId,
                };

                await this.quoteLikesRepository.AddAsync(quoteLike);
                await this.quoteLikesRepository.SaveChangesAsync();
            }

            return OperationResult.Ok(quote.Likes);
        }

        public async Task<OperationResult<int>> UnlikeAsync(
            int quoteId,
            string userId)
        {
            var quote = await this.quoteRepository
                .All()
                .FirstOrDefaultAsync(x => x.Id == quoteId);

            if (quote == null)
            {
                return OperationResult.Fail<int>(QuoteWrongIdError);
            }

            if (quote.UserId == userId)
            {
                return OperationResult.Fail<int>("User cannot like or unlike his or her quotes!");
            }

            var quoteLike = await this.quoteLikesRepository
                .All()
                .FirstOrDefaultAsync(ql => ql.QuoteId == quoteId && ql.UserId == userId);

            if (quoteLike != null)
            {
                quote.Likes = --quote.Likes;
                this.quoteRepository.Update(quote);
                await this.quoteRepository.SaveChangesAsync();

                this.quoteLikesRepository.Delete(quoteLike);
                await this.quoteLikesRepository.SaveChangesAsync();
            }

            return OperationResult.Ok(quote.Likes);
        }
    }
}
