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

        public async Task<Tuple<Stream, string, string>> DownloadBookAsync(int bookId, string userId)
        {
            var book = await this.bookRepository
                .AllAsNoTracking()
                .FirstOrDefaultAsync(b => b.Id == bookId) ??
                throw new InvalidOperationException(BookWrongIdError);

            if (book.IsApproved)
            {
                book.DownloadsCount++;
                this.bookRepository.Update(book);
                await this.bookRepository.SaveChangesAsync();

                await this.usersService.IncreaseUserDailyDownloadsCountAsync(userId);
            }

            return await this.blobService.DownloadBlobAsync(book.FileUrl);
        }
    }
}
