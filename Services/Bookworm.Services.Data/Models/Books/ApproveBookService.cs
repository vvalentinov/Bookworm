namespace Bookworm.Services.Data.Models.Books
{
    using System.Linq;
    using System.Threading.Tasks;

    using Bookworm.Data.Common.Repositories;
    using Bookworm.Data.Models;
    using Bookworm.Services.Data.Contracts.Books;
    using Microsoft.AspNetCore.Identity;

    public class ApproveBookService : IApproveBookService
    {
        private readonly IDeletableEntityRepository<Book> bookRepository;
        private readonly IDeletableEntityRepository<ApplicationUser> userRepository;
        private readonly UserManager<ApplicationUser> userManager;

        public ApproveBookService(
            IDeletableEntityRepository<Book> bookRepository,
            IDeletableEntityRepository<ApplicationUser> userRepository,
            UserManager<ApplicationUser> userManager)
        {
            this.bookRepository = bookRepository;
            this.userRepository = userRepository;
            this.userManager = userManager;
        }

        public async Task ApproveBook(string bookId, string userId)
        {
            ApplicationUser user = this.userRepository.All().First(x => x.Id == userId);
            string email = user.Email;
            await this.userManager.UpdateAsync(user);

            Book book = this.bookRepository.All().First(x => x.Id == bookId);
            book.IsApproved = true;
            this.bookRepository.Update(book);
            await this.bookRepository.SaveChangesAsync();
        }
    }
}
