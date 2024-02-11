namespace Bookworm.Services.Data.Models.Quotes
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    using Bookworm.Common.Enums;
    using Bookworm.Data.Common.Repositories;
    using Bookworm.Data.Models;
    using Bookworm.Services.Data.Contracts;
    using Bookworm.Services.Data.Contracts.Quotes;
    using Bookworm.Services.Messaging;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.EntityFrameworkCore;

    using static Bookworm.Common.GlobalConstants;
    using static Bookworm.Common.PointsDataConstants;
    using static Bookworm.Common.Quotes.QuotesErrorMessagesConstants;

    public class UpdateQuoteService : IUpdateQuoteService
    {
        private readonly IDeletableEntityRepository<Quote> quoteRepository;
        private readonly IDeletableEntityRepository<ApplicationUser> userRepository;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IValidateQuoteService validateQuoteService;
        private readonly IUsersService usersService;
        private readonly IEmailSender emailSender;

        public UpdateQuoteService(
            IDeletableEntityRepository<Quote> quoteRepository,
            IDeletableEntityRepository<ApplicationUser> userRepository,
            UserManager<ApplicationUser> userManager,
            IValidateQuoteService validateQuoteService,
            IUsersService usersService,
            IEmailSender emailSender)
        {
            this.quoteRepository = quoteRepository;
            this.userRepository = userRepository;
            this.userManager = userManager;
            this.validateQuoteService = validateQuoteService;
            this.usersService = usersService;
            this.emailSender = emailSender;
        }

        public async Task ApproveQuoteAsync(int quoteId, string userId)
        {
            ApplicationUser currentUser = await this.userRepository
                .AllAsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == userId);
            bool isCurrUserAdmin = await this.userManager.IsInRoleAsync(currentUser, AdministratorRoleName);
            if (!isCurrUserAdmin)
            {
                throw new InvalidOperationException("You have to be an admin to approve a quote!");
            }

            Quote quote = await this.quoteRepository.All().FirstOrDefaultAsync(x => x.Id == quoteId)
                ?? throw new InvalidOperationException(QuoteWrongIdError);
            quote.IsApproved = true;
            this.quoteRepository.Update(quote);
            await this.quoteRepository.SaveChangesAsync();

            ApplicationUser quoteCreator = await this.userRepository
                .All()
                .FirstOrDefaultAsync(x => x.Id == quote.UserId);
            quoteCreator.Points += QuotePoints;
            this.userRepository.Update(quoteCreator);
            await this.userRepository.SaveChangesAsync();

            // TODO: Send email to user
        }

        public async Task DeleteQuoteAsync(int quoteId, string userId)
        {
            var quote = await this.quoteRepository.All().FirstOrDefaultAsync(q => q.Id == quoteId) ??
                throw new InvalidOperationException(QuoteWrongIdError);

            var user = await this.userRepository.All().FirstOrDefaultAsync(u => u.Id == userId) ??
                throw new InvalidOperationException("No user with given id found!");

            var isUserAdmin = await this.userManager.IsInRoleAsync(user, AdministratorRoleName);

            if (quote.UserId != userId && isUserAdmin == false)
            {
                throw new InvalidOperationException(QuoteDeleteError);
            }

            if (quote.IsApproved)
            {
                await this.usersService.ReduceUserPointsAsync(user, QuotePoints);
            }

            this.quoteRepository.Delete(quote);
            await this.quoteRepository.SaveChangesAsync();
        }

        public async Task UndeleteQuoteAsync(int quoteId)
        {
            Quote quote = this.quoteRepository.AllWithDeleted().First(x => x.Id == quoteId);
            this.quoteRepository.Undelete(quote);
            await this.quoteRepository.SaveChangesAsync();
        }

        public async Task UnapproveQuoteAsync(int quoteId)
        {
            Quote quote = this.quoteRepository.All().First(x => x.Id == quoteId);
            quote.IsApproved = false;
            await this.quoteRepository.SaveChangesAsync();

            ApplicationUser user = this.userRepository.All().First(x => x.Id == quote.UserId);
            if (user.Points > 0)
            {
                user.Points -= QuotePoints;
            }

            await this.userRepository.SaveChangesAsync();
        }

        public async Task EditQuoteAsync(QuoteDto quoteDto, string userId)
        {
            var quote = await this.quoteRepository.All().FirstOrDefaultAsync(q => q.Id == quoteDto.Id) ??
                throw new InvalidOperationException("No quote with given id found!");

            if (quote.UserId != userId)
            {
                throw new InvalidOperationException("You have to be the quote's creator to edit it!");
            }

            bool isValidType = Enum.TryParse(quoteDto.Type, out QuoteType type);

            if (isValidType == false || quote.Type != type)
            {
                throw new InvalidOperationException("Invalid quote type!");
            }

            switch (type)
            {
                case QuoteType.BookQuote:
                    this.validateQuoteService.ValidateBookQuote(quoteDto.Content, quoteDto.BookTitle, quoteDto.AuthorName);
                    quote.AuthorName = quoteDto.AuthorName;
                    quote.BookTitle = quoteDto.BookTitle;
                    break;
                case QuoteType.MovieQuote:
                    this.validateQuoteService.ValidateMovieQuote(quoteDto.Content, quoteDto.MovieTitle);
                    quote.MovieTitle = quoteDto.MovieTitle;
                    break;
                case QuoteType.GeneralQuote:
                    this.validateQuoteService.ValidateGeneralQuote(quoteDto.Content, quoteDto.AuthorName);
                    quote.AuthorName = quoteDto.AuthorName;
                    break;
            }

            quote.Content = quoteDto.Content;

            if (quote.IsApproved)
            {
                var user = await this.userRepository.AllAsNoTracking().FirstAsync(x => x.Id == userId);

                if (user.Points - QuotePoints < 0)
                {
                    user.Points = 0;
                }
                else
                {
                    user.Points -= QuotePoints;
                }

                this.userRepository.Update(user);
                await this.userRepository.SaveChangesAsync();

                quote.IsApproved = false;
            }

            this.quoteRepository.Update(quote);
            await this.quoteRepository.SaveChangesAsync();
        }
    }
}
