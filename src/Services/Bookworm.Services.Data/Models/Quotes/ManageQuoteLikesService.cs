namespace Bookworm.Services.Data.Models.Quotes
{
    using System.Threading.Tasks;

    using Bookworm.Common;
    using Bookworm.Data;
    using Bookworm.Data.Common.Repositories;
    using Bookworm.Data.Models;
    using Bookworm.Services.Data.Contracts.Quotes;
    using Microsoft.EntityFrameworkCore;

    using static Bookworm.Common.Constants.ErrorMessagesConstants.QuoteErrorMessagesConstants;

    public class ManageQuoteLikesService : IManageQuoteLikesService
    {
        private readonly IUnitOfWork unitOfWork;

        private readonly IRepository<QuoteLike> quoteLikesRepo;
        private readonly IDeletableEntityRepository<Quote> quoteRepo;

        public ManageQuoteLikesService(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;

            this.quoteLikesRepo = this.unitOfWork.GetRepository<QuoteLike>();
            this.quoteRepo = this.unitOfWork.GetDeletableEntityRepository<Quote>();
        }

        public async Task<OperationResult<int>> LikeAsync(
            int quoteId,
            string userId)
        {
            var quote = await this.quoteRepo
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

            var quoteLike = await this.quoteLikesRepo
                .AllAsNoTracking()
                .FirstOrDefaultAsync(ql => ql.QuoteId == quoteId && ql.UserId == userId);

            if (quoteLike == null)
            {
                quote.Likes++;
                this.quoteRepo.Update(quote);

                quoteLike = new QuoteLike
                {
                    UserId = userId,
                    QuoteId = quoteId,
                };

                await this.quoteLikesRepo.AddAsync(quoteLike);

                await this.unitOfWork.SaveChangesAsync();
            }

            return OperationResult.Ok(quote.Likes);
        }

        public async Task<OperationResult<int>> UnlikeAsync(
            int quoteId,
            string userId)
        {
            var quote = await this.quoteRepo
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

            var quoteLike = await this.quoteLikesRepo
                .All()
                .FirstOrDefaultAsync(ql => ql.QuoteId == quoteId && ql.UserId == userId);

            if (quoteLike != null)
            {
                quote.Likes--;
                this.quoteRepo.Update(quote);

                this.quoteLikesRepo.Delete(quoteLike);

                await this.unitOfWork.SaveChangesAsync();
            }

            return OperationResult.Ok(quote.Likes);
        }
    }
}
