namespace Bookworm.Services.Data.Models.Books
{
    using System.IO;
    using System.Threading.Tasks;

    using Bookworm.Common;
    using Bookworm.Data.Common.Repositories;
    using Bookworm.Data.Models;
    using Bookworm.Services.Data.Contracts;
    using Bookworm.Services.Data.Contracts.Books;
    using Microsoft.EntityFrameworkCore;

    using static Bookworm.Common.Constants.ErrorMessagesConstants.BookErrorMessagesConstants;
    using static Bookworm.Common.Constants.ErrorMessagesConstants.UserErrorMessagesConstants;

    public class DownloadBookService : IDownloadBookService
    {
        private readonly IBlobService blobService;
        private readonly IUsersService usersService;
        private readonly IDeletableEntityRepository<Book> bookRepository;

        public DownloadBookService(
            IBlobService blobService,
            IUsersService usersService,
            IDeletableEntityRepository<Book> bookRepository)
        {
            this.blobService = blobService;
            this.usersService = usersService;
            this.bookRepository = bookRepository;
        }

        public async Task<OperationResult<(Stream stream, string contentType, string downloadName)>> DownloadBookAsync(
            int bookId,
            ApplicationUser user)
        {
            var isUserAdmin = await this.usersService.IsUserAdminAsync(user.Id);
            var userMaxDailyDownloadsCount = this.usersService.GetUserDailyMaxDownloadsCount(user.Points);

            if (!isUserAdmin && user.DailyDownloadsCount == userMaxDailyDownloadsCount)
            {
                string errMsg = string.Format(UserDailyCountError, userMaxDailyDownloadsCount);

                return OperationResult.Fail<(
                    Stream stream,
                    string contentType,
                    string downloadName)>(errMsg);
            }

            var book = await this.bookRepository
                .AllAsNoTracking()
                .FirstOrDefaultAsync(b => b.Id == bookId);

            if (book == null)
            {
                return OperationResult.Fail<(
                    Stream stream,
                    string contentType,
                    string downloadName)>(BookWrongIdError);
            }

            if (!book.IsApproved)
            {
                return OperationResult.Fail<(
                    Stream stream,
                    string contentType,
                    string downloadName)>(BookNotApprovedError);
            }

            book.DownloadsCount++;
            this.bookRepository.Update(book);
            await this.bookRepository.SaveChangesAsync();

            await this.usersService.IncreaseUserDailyDownloadsCountAsync(user);

            var data = await this.blobService.DownloadBlobAsync(book.FileUrl);

            return OperationResult.Ok(data);
        }
    }
}
