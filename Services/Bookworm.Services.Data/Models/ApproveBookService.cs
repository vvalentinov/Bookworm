namespace Bookworm.Services.Data.Models
{
    using System.Linq;
    using System.Threading.Tasks;

    using Bookworm.Data.Common.Repositories;
    using Bookworm.Data.Models;
    using Bookworm.Services.Data.Contracts;
    using Bookworm.Services.Messaging;
    using Microsoft.AspNetCore.Identity;

    public class ApproveBookService : IApproveBookService
    {
        private readonly IDeletableEntityRepository<Book> bookRepository;
        private readonly IDeletableEntityRepository<ApplicationUser> userRepository;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IEmailSender emailSender;

        public ApproveBookService(
            IDeletableEntityRepository<Book> bookRepository,
            IDeletableEntityRepository<ApplicationUser> userRepository,
            UserManager<ApplicationUser> userManager,
            IEmailSender emailSender)
        {
            this.bookRepository = bookRepository;
            this.userRepository = userRepository;
            this.userManager = userManager;
            this.emailSender = emailSender;
        }

        public async Task ApproveBook(string bookId, string userId)
        {
            var user = this.userRepository.All().First(x => x.Id == userId);
            string email = user.Email;
            user.Points += 5;
            await this.userManager.UpdateAsync(user);

            var book = this.bookRepository.All().First(x => x.Id == bookId);
            book.IsApproved = true;
            this.bookRepository.Update(book);
            await this.bookRepository.SaveChangesAsync();

            await this.emailSender.SendEmailAsync(
                    "bookwormproject@abv.bg",
                    "Bookworm",
                    $"{email}",
                    "Approved Book",
                    $"Congratulations! Your book {book.Title} has been approved by the administrator! You have earned yourself 5 extra points!");
        }
    }
}
