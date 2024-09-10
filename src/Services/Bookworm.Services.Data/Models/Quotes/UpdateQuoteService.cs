namespace Bookworm.Services.Data.Models.Quotes
{
    using System.Threading.Tasks;

    using Bookworm.Common;
    using Bookworm.Common.Hubs;
    using Bookworm.Data;
    using Bookworm.Data.Common.Repositories;
    using Bookworm.Data.Models;
    using Bookworm.Services.Data.Contracts.Quotes;
    using Bookworm.Web.ViewModels.DTOs;
    using Microsoft.AspNetCore.SignalR;
    using Microsoft.EntityFrameworkCore;

    using static Bookworm.Common.Constants.DataConstants.QuoteDataConstants;
    using static Bookworm.Common.Constants.ErrorMessagesConstants.QuoteErrorMessagesConstants;
    using static Bookworm.Common.Constants.NotificationConstants;
    using static Bookworm.Common.Constants.SuccessMessagesConstants.CrudSuccessMessagesConstants;
    using static Bookworm.Common.Constants.TempDataMessageConstant;
    using static Bookworm.Common.Enums.QuoteType;

    public class UpdateQuoteService : IUpdateQuoteService
    {
        private readonly IUnitOfWork unitOfWork;

        private readonly IHubContext<NotificationHub> notificationHub;

        private readonly IDeletableEntityRepository<Quote> quoteRepo;
        private readonly IDeletableEntityRepository<ApplicationUser> userRepo;
        private readonly IDeletableEntityRepository<Notification> notificationRepo;

        public UpdateQuoteService(
            IUnitOfWork unitOfWork,
            IHubContext<NotificationHub> notificationHub)
        {
            this.unitOfWork = unitOfWork;

            this.notificationHub = notificationHub;

            this.quoteRepo = this.unitOfWork.GetDeletableEntityRepository<Quote>();
            this.userRepo = this.unitOfWork.GetDeletableEntityRepository<ApplicationUser>();
            this.notificationRepo = this.unitOfWork.GetDeletableEntityRepository<Notification>();
        }

        public async Task<OperationResult> ApproveQuoteAsync(int quoteId)
        {
            var quote = await this.quoteRepo
                .All()
                .FirstOrDefaultAsync(x => x.Id == quoteId);

            if (quote == null)
            {
                return OperationResult.Fail(QuoteWrongIdError);
            }

            if (!quote.IsApproved)
            {
                this.quoteRepo.Approve(quote);

                var user = await this.userRepo
                    .All()
                    .FirstAsync(x => x.Id == quote.UserId);

                user.Points += QuoteUploadPoints;

                this.userRepo.Update(user);

                var notificationContent = string.Format(
                    ApprovedQuoteNotification,
                    quote.Content,
                    QuoteUploadPoints);

                var notification = new Notification
                {
                    UserId = quote.UserId,
                    Content = notificationContent,
                };

                await this.notificationRepo.AddAsync(notification);

                await this.unitOfWork.SaveChangesAsync();

                await this.notificationHub
                    .Clients
                    .User(quote.UserId)
                    .SendAsync("notify", ApprovedQuoteMessage);
            }

            return OperationResult.Ok();
        }

        public async Task<OperationResult> DeleteQuoteAsync(
            int quoteId,
            string userId,
            bool isCurrUserAdmin = false)
        {
            var quote = await this.quoteRepo
                .All()
                .FirstOrDefaultAsync(q => q.Id == quoteId);

            if (quote == null)
            {
                return OperationResult.Fail(QuoteWrongIdError);
            }

            if (!isCurrUserAdmin && quote.UserId != userId)
            {
                return OperationResult.Fail(QuoteDeleteError);
            }

            if (quote.IsApproved)
            {
                var user = await this.userRepo
                    .All()
                    .FirstAsync(x => x.Id == quote.UserId);

                user.Points -= QuoteUploadPoints;
                this.userRepo.Update(user);
            }

            this.quoteRepo.Delete(quote);

            await this.unitOfWork.SaveChangesAsync();

            return OperationResult.Ok(DeleteSuccess);
        }

        public async Task<OperationResult> UndeleteQuoteAsync(int quoteId)
        {
            var quote = await this.quoteRepo
                .AllWithDeleted()
                .FirstOrDefaultAsync(x => x.Id == quoteId);

            if (quote == null)
            {
                return OperationResult.Fail(QuoteWrongIdError);
            }

            if (quote.IsDeleted)
            {
                this.quoteRepo.Undelete(quote);
                await this.quoteRepo.SaveChangesAsync();
            }

            return OperationResult.Ok();
        }

        public async Task<OperationResult> UnapproveQuoteAsync(int quoteId)
        {
            var quote = await this.quoteRepo
                .All()
                .FirstOrDefaultAsync(x => x.Id == quoteId);

            if (quote == null)
            {
                return OperationResult.Fail(QuoteWrongIdError);
            }

            if (quote.IsApproved)
            {
                this.quoteRepo.Unapprove(quote);

                var user = await this.userRepo
                    .All()
                    .FirstAsync(x => x.Id == quote.UserId);

                user.Points -= QuoteUploadPoints;
                this.userRepo.Update(user);

                var notificationContent = string.Format(
                    UnapprovedQuoteNotification,
                    quote.Content,
                    QuoteUploadPoints);

                var notification = new Notification
                {
                    UserId = quote.UserId,
                    Content = notificationContent,
                };

                await this.notificationRepo.AddAsync(notification);

                await this.unitOfWork.SaveChangesAsync();

                await this.notificationHub
                    .Clients
                    .User(quote.UserId)
                    .SendAsync("notify", UnapprovedQuoteMessage);
            }

            return OperationResult.Ok();
        }

        public async Task<OperationResult> EditQuoteAsync(
            QuoteDto quoteDto,
            string userId)
        {
            var quote = await this.quoteRepo
                .All()
                .FirstOrDefaultAsync(q => q.Id == quoteDto.Id);

            if (quote == null)
            {
                return OperationResult.Fail(QuoteWrongIdError);
            }

            if (quote.UserId != userId)
            {
                return OperationResult.Fail(QuoteEditError);
            }

            if (quote.Type != quoteDto.Type)
            {
                return OperationResult.Fail(QuoteInvalidTypeError);
            }

            quote.Content = quoteDto.Content;

            switch (quoteDto.Type)
            {
                case BookQuote:
                    quote.AuthorName = quoteDto.AuthorName;
                    quote.BookTitle = quoteDto.BookTitle;
                    break;
                case MovieQuote:
                    quote.MovieTitle = quoteDto.MovieTitle;
                    break;
                case GeneralQuote:
                    quote.AuthorName = quoteDto.AuthorName;
                    break;
            }

            if (quote.IsApproved)
            {
                quote.IsApproved = false;

                var user = await this.userRepo
                    .All()
                    .FirstAsync(x => x.Id == userId);

                user.Points -= QuoteUploadPoints;
                this.userRepo.Update(user);
            }

            this.quoteRepo.Update(quote);

            await this.unitOfWork.SaveChangesAsync();

            return OperationResult.Ok(EditSuccess);
        }
    }
}
