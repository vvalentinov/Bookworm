namespace Bookworm.Services.Data.Models
{
    using System.Linq;
    using System.Threading.Tasks;

    using Bookworm.Data.Common.Repositories;
    using Bookworm.Data.Models;
    using Bookworm.Services.Data.Contracts;

    public class DeleteBookService : IDeleteBookService
    {
        private readonly IDeletableEntityRepository<Book> bookRepository;
        private readonly IBlobService blobService;

        public DeleteBookService(
            IDeletableEntityRepository<Book> bookRepository,
            IBlobService blobService)
        {
            this.bookRepository = bookRepository;
            this.blobService = blobService;
        }

        public async Task DeleteBookAsync(string bookId)
        {
            var book = this.bookRepository.All().First(x => x.Id == bookId);
            var blob = this.blobService.GetBlobClient(book.FileUrl);

            if (blob != null)
            {
                await this.blobService.DeleteBlobAsync(blob.Name);
            }

            this.bookRepository.Delete(book);
            await this.bookRepository.SaveChangesAsync();
        }
    }
}
