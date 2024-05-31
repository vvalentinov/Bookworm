namespace Bookworm.Services.Data.Models.Books
{
    using System;
    using System.IO;
    using System.Threading.Tasks;

    using Bookworm.Data.Common.Repositories;
    using Bookworm.Data.Models;
    using Bookworm.Services.Data.Contracts;
    using Bookworm.Services.Data.Contracts.Books;
    using Microsoft.EntityFrameworkCore;

    using static Bookworm.Common.Constants.ErrorMessagesConstants.BookErrorMessagesConstants;
    using static Bookworm.Common.Constants.ErrorMessagesConstants.UserErrorMessagesConstants;

    public class DownloadBookService : IDownloadBookService
    {
        private readonly IDeletableEntityRepository<Book> bookRepository;
        private readonly IBlobService blobService;
        private readonly IUsersService usersService;

        public DownloadBookService(
            IDeletableEntityRepository<Book> bookRepository,
            IBlobService blobService,
            IUsersService usersService)
        {
            this.bookRepository = bookRepository;
            this.blobService = blobService;
            this.usersService = usersService;
        }

        public async Task<Tuple<Stream, string, string>> DownloadBookAsync(int bookId, ApplicationUser user)
        {
            var isUserAdmin = await this.usersService.IsUserAdminAsync(user.Id);
            var userMaxDailyDownloadsCount = this.usersService.GetUserDailyMaxDownloadsCount(user.Points);

            if (user.DailyDownloadsCount == userMaxDailyDownloadsCount && !isUserAdmin)
            {
                string errMsg = string.Format(UserDailyCountError, userMaxDailyDownloadsCount);
                throw new InvalidOperationException(errMsg);
            }

            var book = await this.bookRepository
                .AllAsNoTracking()
                .FirstOrDefaultAsync(b => b.Id == bookId) ??
                throw new InvalidOperationException(BookWrongIdError);

            if (!book.IsApproved)
            {
                throw new InvalidOperationException(BookNotApprovedError);
            }

            book.DownloadsCount++;
            this.bookRepository.Update(book);
            await this.bookRepository.SaveChangesAsync();

            await this.usersService.IncreaseUserDailyDownloadsCountAsync(user);

            return await this.blobService.DownloadBlobAsync(book.FileUrl);
        }
    }
}
