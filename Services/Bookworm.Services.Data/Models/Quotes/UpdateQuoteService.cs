namespace Bookworm.Services.Data.Models.Quotes
{
    using System.Linq;
    using System.Threading.Tasks;

    using Bookworm.Data.Common.Repositories;
    using Bookworm.Data.Models;
    using Bookworm.Services.Data.Contracts.Quotes;
    using Bookworm.Services.Messaging;
    using Microsoft.AspNetCore.Identity;

    public class UpdateQuoteService : IUpdateQuoteService
    {
        private readonly IRepository<Quote> quoteRepository;
        private readonly IEmailSender emailSender;
        private readonly IDeletableEntityRepository<ApplicationUser> userRepository;
        private readonly UserManager<ApplicationUser> userManager;

        public UpdateQuoteService(
            IRepository<Quote> quoteRepository,
            IEmailSender emailSender,
            IDeletableEntityRepository<ApplicationUser> userRepository,
            UserManager<ApplicationUser> userManager)
        {
            this.quoteRepository = quoteRepository;
            this.emailSender = emailSender;
            this.userRepository = userRepository;
            this.userManager = userManager;
        }

        public async Task ApproveQuote(int id, string userId)
        {
            var user = this.userRepository.All().First(x => x.Id == userId);
            string email = user.Email;

            var quote = this.quoteRepository.All().First(x => x.Id == id);
            quote.IsApproved = true;
            this.quoteRepository.Update(quote);
            await this.quoteRepository.SaveChangesAsync();
            user.Points += 3;
            await this.userManager.UpdateAsync(user);

            await this.emailSender.SendEmailAsync(
                    "bookwormproject@abv.bg",
                    "Bookworm",
                    $"{email}",
                    "Approved Quote",
                    $"Congratulations! Your quote {quote.Content} has been approved by the administrator! You have earned yourself 3 extra points!");
        }

        public async Task DeleteQuoteAsync(int quoteId)
        {
            var quote = this.quoteRepository.All().First(x => x.Id == quoteId);
            quote.IsDeleted = true;
            await this.quoteRepository.SaveChangesAsync();
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
    }
}
