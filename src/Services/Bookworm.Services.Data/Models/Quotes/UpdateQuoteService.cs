namespace Bookworm.Services.Data.Models.Quotes
{
    using System.Threading.Tasks;

    using Bookworm.Common;
    using Bookworm.Data.Common.Repositories;
    using Bookworm.Data.Models;
    using Bookworm.Services.Data.Contracts;
    using Bookworm.Services.Data.Contracts.Quotes;
    using Bookworm.Web.ViewModels.DTOs;
    using Microsoft.EntityFrameworkCore;

    using static Bookworm.Common.Constants.DataConstants.QuoteDataConstants;
    using static Bookworm.Common.Constants.ErrorMessagesConstants.QuoteErrorMessagesConstants;
    using static Bookworm.Common.Constants.NotificationConstants;
    using static Bookworm.Common.Constants.SuccessMessagesConstants.CrudSuccessMessagesConstants;
    using static Bookworm.Common.Enums.QuoteType;

    public class UpdateQuoteService : IUpdateQuoteService
    {
        private readonly IUsersService usersService;
        private readonly INotificationService notificationService;

        // private readonly IHubContext<NotificationHub> notificationHub;
        private readonly IDeletableEntityRepository<Quote> quoteRepository;

        public UpdateQuoteService(
            IUsersService usersService,
            INotificationService notificationService,
            IDeletableEntityRepository<Quote> quoteRepository)
        {
            this.usersService = usersService;
            this.quoteRepository = quoteRepository;

            // this.notificationHub = notificationHub;
            this.notificationService = notificationService;
        }

        public async Task<OperationResult> ApproveQuoteAsync(int quoteId)
        {
            var quote = await this.quoteRepository
                .All()
                .FirstOrDefaultAsync(x => x.Id == quoteId);

            if (quote == null)
            {
                return OperationResult.Fail(QuoteWrongIdError);
            }

            if (!quote.IsApproved)
            {
                quote.IsApproved = true;
                this.quoteRepository.Update(quote);
                await this.quoteRepository.SaveChangesAsync();

                await this.usersService.IncreaseUserPointsAsync(
                    quote.UserId,
                    QuoteUploadPoints);

                var notificationContent = string.Format(
                    ApprovedQuoteNotification,
                    quote.Content,
                    QuoteUploadPoints);

                await this.notificationService.AddNotificationAsync(
                    notificationContent,
                    quote.UserId);

                // await this.notificationHub.Clients.User(quote.UserId).SendAsync("notify", ApprovedQuoteMessage);
            }

            return OperationResult.Ok();
        }

        public async Task<OperationResult> DeleteQuoteAsync(
            int quoteId,
            string userId,
            bool isCurrUserAdmin = false)
        {
            var quote = await this.quoteRepository
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
                await this.usersService.ReduceUserPointsAsync(
                    quote.UserId,
                    QuoteUploadPoints);
            }

            this.quoteRepository.Delete(quote);
            await this.quoteRepository.SaveChangesAsync();

            return OperationResult.Ok(DeleteSuccess);
        }

        public async Task<OperationResult> UndeleteQuoteAsync(int quoteId)
        {
            var quote = await this.quoteRepository
                .AllWithDeleted()
                .FirstOrDefaultAsync(x => x.Id == quoteId);

            if (quote == null)
            {
                return OperationResult.Fail(QuoteWrongIdError);
            }

            if (quote.IsDeleted)
            {
                this.quoteRepository.Undelete(quote);
                await this.quoteRepository.SaveChangesAsync();
            }

            return OperationResult.Ok();
        }

        public async Task<OperationResult> UnapproveQuoteAsync(int quoteId)
        {
            var quote = await this.quoteRepository
                .All()
                .FirstOrDefaultAsync(x => x.Id == quoteId);

            if (quote == null)
            {
                return OperationResult.Fail(QuoteWrongIdError);
            }

            if (quote.IsApproved)
            {
                quote.IsApproved = false;
                await this.quoteRepository.SaveChangesAsync();

                await this.usersService.ReduceUserPointsAsync(
                    quote.UserId,
                    QuoteUploadPoints);

                var notificationContent = string.Format(
                    UnapprovedQuoteNotification,
                    quote.Content,
                    QuoteUploadPoints);

                await this.notificationService.AddNotificationAsync(
                    notificationContent,
                    quote.UserId);

                // await this.notificationHub.Clients.User(quote.UserId).SendAsync("notify", UnapprovedQuoteMessage);
            }

            return OperationResult.Ok();
        }

        public async Task<OperationResult> EditQuoteAsync(
            QuoteDto quoteDto,
            string userId)
        {
            var quote = await this.quoteRepository
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

                await this.usersService.ReduceUserPointsAsync(
                    userId,
                    QuoteUploadPoints);
            }

            this.quoteRepository.Update(quote);
            await this.quoteRepository.SaveChangesAsync();

            return OperationResult.Ok(EditSuccess);
        }
    }
}
