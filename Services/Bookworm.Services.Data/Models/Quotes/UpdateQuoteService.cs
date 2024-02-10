namespace Bookworm.Services.Data.Models.Quotes
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    using Bookworm.Common.Enums;
    using Bookworm.Data.Common.Repositories;
    using Bookworm.Data.Models;
    using Bookworm.Services.Data.Contracts.Quotes;
    using Bookworm.Services.Messaging;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.EntityFrameworkCore;

    using static Bookworm.Common.GlobalConstants;
    using static Bookworm.Common.PointsDataConstants;

    public class UpdateQuoteService : IUpdateQuoteService
    {
        private readonly IDeletableEntityRepository<Quote> quoteRepository;
        private readonly IDeletableEntityRepository<ApplicationUser> userRepository;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IValidateQuoteService validateQuoteService;
        private readonly IEmailSender emailSender;

        public UpdateQuoteService(
            IDeletableEntityRepository<Quote> quoteRepository,
            IDeletableEntityRepository<ApplicationUser> userRepository,
            UserManager<ApplicationUser> userManager,
            IValidateQuoteService validateQuoteService,
            IEmailSender emailSender)
        {
            this.quoteRepository = quoteRepository;
            this.userRepository = userRepository;
            this.userManager = userManager;
            this.validateQuoteService = validateQuoteService;
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
                ?? throw new InvalidOperationException("No quote with given id found!");
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

        public async Task DeleteQuoteAsync(int quoteId)
        {
            await this.DeleteQuote(quoteId);
        }

        public async Task SelfQuoteDeleteAsync(int quoteId, string userId)
        {
            await this.DeleteQuote(quoteId);
            ApplicationUser user = await this.userRepository.All().FirstAsync(x => x.Id == userId);
            if (user.Points > 0)
            {
                user.Points -= QuotePoints;
            }

            await this.userRepository.SaveChangesAsync();
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

        public async Task EditQuoteAsync(QuoteDto quote, string userId)
        {
            var dbQuote = await this.quoteRepository
                .All()
                .FirstOrDefaultAsync(q => q.Id == quote.Id) ??
                throw new InvalidOperationException("No quote with given id found!");

            if (dbQuote.UserId != userId)
            {
                throw new InvalidOperationException("You have to be the quote's creator to edit it!");
            }

            if (dbQuote.Type != quote.Type)
            {
                throw new InvalidOperationException("Invalid quote type!");
            }

            switch (quote.Type)
            {
                case QuoteType.BookQuote:
                    this.validateQuoteService.ValidateBookQuote(quote.Content, quote.BookTitle, quote.AuthorName);
                    dbQuote.AuthorName = quote.AuthorName;
                    dbQuote.BookTitle = quote.BookTitle;
                    break;
                case QuoteType.MovieQuote:
                    this.validateQuoteService.ValidateMovieQuote(quote.Content, quote.MovieTitle);
                    dbQuote.MovieTitle = quote.MovieTitle;
                    break;
                case QuoteType.GeneralQuote:
                    this.validateQuoteService.ValidateGeneralQuote(quote.Content, quote.AuthorName);
                    dbQuote.AuthorName = quote.AuthorName;
                    break;
            }

            dbQuote.Content = quote.Content;

            if (dbQuote.IsApproved)
            {
                var user = await this.userRepository
                    .AllAsNoTracking()
                    .FirstAsync(x => x.Id == userId);

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
            }

            dbQuote.IsApproved = false;

            this.quoteRepository.Update(dbQuote);
            await this.quoteRepository.SaveChangesAsync();
        }

        private async Task DeleteQuote(int quoteId)
        {
            Quote quote = this.quoteRepository.All().First(x => x.Id == quoteId);
            this.quoteRepository.Delete(quote);
            quote.IsApproved = false;
            await this.quoteRepository.SaveChangesAsync();
        }
    }
}
