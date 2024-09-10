namespace Bookworm.Services.Data.Models.Books
{
    using System;
    using System.IO;
    using System.Threading.Tasks;

    using Bookworm.Common;
    using Bookworm.Data;
    using Bookworm.Data.Common.Repositories;
    using Bookworm.Data.Models;
    using Bookworm.Services.Data.Contracts;
    using Bookworm.Services.Data.Contracts.Books;
    using Microsoft.EntityFrameworkCore;

    using static Bookworm.Common.Constants.ErrorMessagesConstants.BookErrorMessagesConstants;
    using static Bookworm.Common.Constants.ErrorMessagesConstants.UserErrorMessagesConstants;

    public class DownloadBookService : IDownloadBookService
    {
        private readonly IUnitOfWork unitOfWork;

        private readonly IBlobService blobService;
        private readonly IUsersService usersService;

        private readonly IRepository<Book> bookRepo;
        private readonly IRepository<ApplicationUser> userRepo;

        public DownloadBookService(
            IUnitOfWork unitOfWork,
            IBlobService blobService,
            IUsersService usersService)
        {
            this.unitOfWork = unitOfWork;

            this.blobService = blobService;
            this.usersService = usersService;

            this.bookRepo = this.unitOfWork.GetRepository<Book>();
            this.userRepo = this.unitOfWork.GetRepository<ApplicationUser>();
        }

        public async Task<OperationResult<(Stream stream, string contentType, string downloadName)>> DownloadBookAsync(
             int bookId,
             bool isUserAdmin,
             string userId)
        {
            var user = await this.userRepo
                .All()
                .FirstAsync(x => x.Id == userId);

            var userMaxDailyDownloadsCount = this.usersService
                .GetUserDailyMaxDownloadsCount(user.Points);

            if (!isUserAdmin && user.DailyDownloadsCount == userMaxDailyDownloadsCount)
            {
                string errorMessage = string.Format(
                    UserDailyCountError,
                    userMaxDailyDownloadsCount);

                return OperationResult.Fail<(
                    Stream stream,
                    string contentType,
                    string downloadName)>(errorMessage);
            }

            var book = await this.bookRepo
                .All()
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

            var data = await this.blobService.DownloadBlobAsync(book.FileUrl);

            using var transaction = await this.unitOfWork.BeginTransactionAsync();

            try
            {
                book.DownloadsCount++;
                this.bookRepo.Update(book);

                if (user.DailyDownloadsCount < userMaxDailyDownloadsCount)
                {
                    user.DailyDownloadsCount++;
                    this.userRepo.Update(user);
                }

                await this.unitOfWork.SaveChangesAsync();

                await this.unitOfWork.CommitTransactionAsync();

                return OperationResult.Ok(data);
            }
            catch (Exception)
            {
                await this.unitOfWork.RollbackTransactionAsync();

                return OperationResult.Fail<(
                    Stream stream,
                    string contentType,
                    string downloadName)>("Problem when updating entities in database!");
            }
        }
    }
}
