namespace Bookworm.Services.Data.Models.Books
{
    using System.Linq;
    using System.Threading.Tasks;

    using Bookworm.Data.Common.Repositories;
    using Bookworm.Data.Models;
    using Bookworm.Services.Data.Contracts.Books;
    using Bookworm.Web.ViewModels.Books;
    using Microsoft.EntityFrameworkCore;

    using static Bookworm.Common.Books.BooksDataConstants;

    public class SearchBooksService : ISearchBooksService
    {
        private readonly IDeletableEntityRepository<Book> bookRepository;

        public SearchBooksService(IDeletableEntityRepository<Book> bookRepository)
        {
            this.bookRepository = bookRepository;
        }

        public async Task<BookListingViewModel> SearchBooks(string input, int page, int categoryId)
            => new BookListingViewModel
            {
                Books = await this.bookRepository
                        .AllAsNoTracking()
                        .Include(b => b.Publisher)
                        .Include(b => b.AuthorsBooks)
                        .ThenInclude(b => b.Author)
                        .Where(b => b.CategoryId == categoryId && b.IsApproved &&
                            (b.Title.Contains(input) ||
                            b.Publisher.Name.Contains(input) ||
                            b.AuthorsBooks.Select(b => b.Author).Any(x => x.Name.Contains(input))))
                        .OrderByDescending(b => b.CreatedOn)
                        .Select(x => new BookViewModel { Id = x.Id, Title = x.Title, ImageUrl = x.ImageUrl })
                        .Skip((page - 1) * BooksPerPage)
                        .Take(BooksPerPage)
                        .ToListAsync(),
                RecordsCount = await this.bookRepository
                        .AllAsNoTracking()
                        .Include(b => b.Publisher)
                        .Include(b => b.AuthorsBooks)
                        .ThenInclude(b => b.Author)
                        .Where(x => x.AuthorsBooks.Any(ab => ab.Author.Name.Contains(input) ||
                               x.Publisher.Name.Contains(input) ||
                               x.Title.Contains(input))).CountAsync(),
                PageNumber = page,
                ItemsPerPage = BooksPerPage,
            };

        public async Task<BookListingViewModel> SearchUserBooks(string input, int page, string userId)
            => new BookListingViewModel
            {
                Books = await this.bookRepository
                        .AllAsNoTracking()
                        .Include(b => b.Publisher)
                        .Include(b => b.AuthorsBooks)
                        .ThenInclude(b => b.Author)
                        .Where(x => x.UserId == userId &&
                            (x.Title.Contains(input) ||
                            x.Publisher.Name.Contains(input) ||
                            x.AuthorsBooks.Select(b => b.Author).Any(x => x.Name.Contains(input))))
                        .OrderByDescending(b => b.CreatedOn)
                        .Select(x => new BookViewModel { Id = x.Id, Title = x.Title, ImageUrl = x.ImageUrl })
                        .Skip((page - 1) * BooksPerPage)
                        .Take(BooksPerPage)
                        .ToListAsync(),
                RecordsCount = await this.bookRepository
                        .AllAsNoTracking()
                        .Include(b => b.Publisher)
                        .Include(b => b.AuthorsBooks)
                        .ThenInclude(b => b.Author)
                        .Where(x => x.AuthorsBooks.Any(ab => ab.Author.Name.Contains(input) ||
                               x.Publisher.Name.Contains(input) ||
                               x.Title.Contains(input))).CountAsync(),
                PageNumber = page,
                ItemsPerPage = BooksPerPage,
            };
    }
}
