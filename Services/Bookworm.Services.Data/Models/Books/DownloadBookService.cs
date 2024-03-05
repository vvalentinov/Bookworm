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

    using static Bookworm.Common.Books.BooksErrorMessagesConstants;

    public class DownloadBookService : IDownloadBookService
    {
        private readonly IDeletableEntityRepository<Book> bookRepository;
        private readonly IBlobService blobService;

        public DownloadBookService(
            IDeletableEntityRepository<Book> bookRepository,
            IBlobService blobService)
        {
            this.bookRepository = bookRepository;
            this.blobService = blobService;
        }

        public async Task<Tuple<Stream, string, string>> DownloadBookAsync(int bookId)
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
            }

            return await this.blobService.DownloadBlobAsync(book.FileUrl);
        }
    }
}
