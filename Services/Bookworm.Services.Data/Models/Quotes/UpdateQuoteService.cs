namespace Bookworm.Services.Data.Models.Quotes
{
    using System;
    using System.Threading.Tasks;

    using Bookworm.Data.Common.Repositories;
    using Bookworm.Data.Models;
    using Bookworm.Services.Data.Contracts;
    using Bookworm.Services.Data.Contracts.Quotes;
    using Bookworm.Services.Messaging;
    using Bookworm.Web.ViewModels.DTOs;
    using Microsoft.EntityFrameworkCore;

    using static Bookworm.Common.Constants.DataConstants.QuoteDataConstants;
    using static Bookworm.Common.Constants.ErrorMessagesConstants.QuoteErrorMessagesConstants;
    using static Bookworm.Common.Enums.QuoteType;

    public class UpdateQuoteService : IUpdateQuoteService
    {
        private readonly IDeletableEntityRepository<Quote> quoteRepository;
        private readonly IUsersService usersService;

        public UpdateQuoteService(
            IDeletableEntityRepository<Quote> quoteRepository,
            IUsersService usersService)
        {
            this.quoteRepository = quoteRepository;
            this.usersService = usersService;
        }

        public async Task ApproveQuoteAsync(int quoteId)
        {
            var quote = await this.quoteRepository.All()
                .FirstOrDefaultAsync(x => x.Id == quoteId) ??
                throw new InvalidOperationException(QuoteWrongIdError);

            if (quote.IsApproved == false)
            {
                quote.IsApproved = true;
                this.quoteRepository.Update(quote);
                await this.quoteRepository.SaveChangesAsync();

                await this.usersService.IncreaseUserPointsAsync(quote.UserId, QuoteUploadPoints);
            }
        }

        public async Task DeleteQuoteAsync(
            int quoteId,
            string userId,
            bool isCurrUserAdmin = false)
        {
            var quote = await this.quoteRepository
                .All()
                .FirstOrDefaultAsync(q => q.Id == quoteId) ??
                throw new InvalidOperationException(QuoteWrongIdError);

            if (quote.UserId != userId && isCurrUserAdmin == false)
            {
                throw new InvalidOperationException(QuoteDeleteError);
            }

            if (quote.IsApproved)
            {
                await this.usersService.ReduceUserPointsAsync(quote.UserId, QuoteUploadPoints);
            }

            this.quoteRepository.Delete(quote);
            await this.quoteRepository.SaveChangesAsync();
        }

        public async Task UndeleteQuoteAsync(int quoteId)
        {
            var quote = await this.quoteRepository
                .AllWithDeleted()
                .FirstOrDefaultAsync(x => x.Id == quoteId)
                ?? throw new InvalidOperationException(QuoteWrongIdError);

            if (quote.IsDeleted)
            {
                this.quoteRepository.Undelete(quote);
                await this.quoteRepository.SaveChangesAsync();
            }
        }

        public async Task UnapproveQuoteAsync(int quoteId)
        {
            var quote = await this.quoteRepository.All()
                .FirstOrDefaultAsync(x => x.Id == quoteId) ??
                throw new InvalidOperationException(QuoteWrongIdError);

            if (quote.IsApproved)
            {
                quote.IsApproved = false;
                await this.quoteRepository.SaveChangesAsync();
                await this.usersService.ReduceUserPointsAsync(quote.UserId, QuoteUploadPoints);
            }
        }

        public async Task EditQuoteAsync(QuoteDto quoteDto, string userId)
        {
            var quote = await this.quoteRepository.All()
                .FirstOrDefaultAsync(q => q.Id == quoteDto.Id) ??
                throw new InvalidOperationException(QuoteWrongIdError);

            if (quote.UserId != userId)
            {
                throw new InvalidOperationException(QuoteEditError);
            }

            if (quote.Type != quoteDto.Type)
            {
                throw new InvalidOperationException(QuoteInvalidTypeError);
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
                await this.usersService.ReduceUserPointsAsync(userId, QuoteUploadPoints);
                quote.IsApproved = false;
            }

            this.quoteRepository.Update(quote);
            await this.quoteRepository.SaveChangesAsync();
        }
    }
}
