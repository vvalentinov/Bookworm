namespace Bookworm.Services.Data.Models.Quotes
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    using Bookworm.Data.Common.Repositories;
    using Bookworm.Data.Models;
    using Bookworm.Services.Data.Contracts.Quotes;
    using Bookworm.Services.Messaging;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.EntityFrameworkCore;

    using static Bookworm.Common.GlobalConstants;
    using static Bookworm.Common.UsersPointsDataConstants;

    public class UpdateQuoteService : IUpdateQuoteService
    {
        private readonly IDeletableEntityRepository<Quote> quoteRepository;
        private readonly IDeletableEntityRepository<ApplicationUser> userRepository;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IEmailSender emailSender;

        public UpdateQuoteService(
            IDeletableEntityRepository<Quote> quoteRepository,
            IDeletableEntityRepository<ApplicationUser> userRepository,
            UserManager<ApplicationUser> userManager,
            IEmailSender emailSender)
        {
            this.quoteRepository = quoteRepository;
            this.userRepository = userRepository;
            this.userManager = userManager;
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

        public async Task EditQuoteAsync(
            int quoteId,
            string content,
            string authorName,
            string bookTitle,
            string movieTitle)
        {
            Quote quote = this.quoteRepository.All().First(x => x.Id == quoteId);
            quote.Content = content;
            quote.AuthorName = authorName;
            quote.BookTitle = bookTitle;
            quote.MovieTitle = movieTitle;

            this.quoteRepository.Update(quote);
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
