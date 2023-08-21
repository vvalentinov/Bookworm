namespace Bookworm.Services.Data.Models.Quotes
{
    using System.Linq;
    using System.Threading.Tasks;

    using Bookworm.Data.Common.Repositories;
    using Bookworm.Data.Models;
    using Bookworm.Services.Data.Contracts.Quotes;
    using Microsoft.AspNetCore.Identity;

    public class UpdateQuoteService : IUpdateQuoteService
    {
        private readonly IDeletableEntityRepository<Quote> quoteRepository;
        private readonly IDeletableEntityRepository<ApplicationUser> userRepository;
        private readonly UserManager<ApplicationUser> userManager;

        public UpdateQuoteService(
            IDeletableEntityRepository<Quote> quoteRepository,
            IDeletableEntityRepository<ApplicationUser> userRepository,
            UserManager<ApplicationUser> userManager)
        {
            this.quoteRepository = quoteRepository;
            this.userRepository = userRepository;
            this.userManager = userManager;
        }

        public async Task ApproveQuoteAsync(int id, string userId)
        {
            ApplicationUser user = this.userRepository.All().First(x => x.Id == userId);

            Quote quote = this.quoteRepository.All().First(x => x.Id == id);
            quote.IsApproved = true;
            quote.IsDeleted = false;
            this.quoteRepository.Update(quote);
            await this.quoteRepository.SaveChangesAsync();

            user.Points += 3;
            await this.userManager.UpdateAsync(user);
        }

        public async Task DeleteQuoteAsync(int quoteId)
        {
            Quote quote = this.quoteRepository.All().First(x => x.Id == quoteId);
            quote.IsDeleted = true;
            quote.IsApproved = false;
            await this.quoteRepository.SaveChangesAsync();
        }

        public async Task UndeleteQuoteAsync(int quoteId)
        {
            Quote quote = this.quoteRepository.AllWithDeleted().First(x => x.Id == quoteId);
            quote.IsDeleted = false;
            await this.quoteRepository.SaveChangesAsync();
        }

        public async Task UnapproveQuoteAsync(int quoteId)
        {
            Quote quote = this.quoteRepository.All().First(x => x.Id == quoteId);
            quote.IsApproved = false;
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
