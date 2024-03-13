namespace Bookworm.Services.Data.Models.Books
{
    using System.Linq;
    using System.Threading.Tasks;

    using Bookworm.Data.Common.Repositories;
    using Bookworm.Data.Models;
    using Bookworm.Services.Data.Contracts.Books;
    using Bookworm.Web.ViewModels.Books;
    using Bookworm.Web.ViewModels.Books.ListingViewModels;
    using Microsoft.EntityFrameworkCore;

    using static Bookworm.Common.Books.BooksDataConstants;

    public class SearchBooksService : ISearchBooksService
    {
        private readonly IDeletableEntityRepository<Book> bookRepository;

        public SearchBooksService(IDeletableEntityRepository<Book> bookRepository)
        {
            this.bookRepository = bookRepository;
        }

        public async Task<BookListingViewModel> SearchBooks(SearchBookInputModel model)
        {
            var booksQuery = (IQueryable<Book>)this.bookRepository
                .AllWithDeleted()
                .Include(b => b.Publisher)
                .Include(b => b.AuthorsBooks)
                .ThenInclude(b => b.Author);

            var recordsCountQuery = (IQueryable<Book>)this.bookRepository
                        .AllAsNoTracking()
                        .Include(b => b.Publisher)
                        .Include(b => b.AuthorsBooks)
                        .ThenInclude(b => b.Author);

            int recordsCount;

            if (model.LanguagesIds.Count > 0)
            {
                booksQuery = booksQuery.Where(b => model.LanguagesIds.Contains(b.LanguageId));
                recordsCountQuery = recordsCountQuery.Where(b => model.LanguagesIds.Contains(b.LanguageId));
            }

            if (model.IsForUserBooks)
            {
                booksQuery = booksQuery.Where(x =>
                            x.UserId == model.UserId &&
                            (x.Title.Contains(model.Input) ||
                            x.Publisher.Name.Contains(model.Input) ||
                            x.AuthorsBooks.Select(b => b.Author).Any(x => x.Name.Contains(model.Input))));

                recordsCount = await recordsCountQuery.Where(x =>
                            x.UserId == model.UserId &&
                            (x.Title.Contains(model.Input) ||
                            x.Publisher.Name.Contains(model.Input) ||
                            x.AuthorsBooks.Select(b => b.Author).Any(x => x.Name.Contains(model.Input))))
                            .CountAsync();
            }
            else
            {
                booksQuery = booksQuery.Where(b =>
                            b.IsApproved &&
                            b.CategoryId == model.CategoryId &&
                            (b.Title.Contains(model.Input) ||
                            b.Publisher.Name.Contains(model.Input) ||
                            b.AuthorsBooks.Select(b => b.Author).Any(x => x.Name.Contains(model.Input))));

                recordsCount = await recordsCountQuery.Where(b =>
                            b.CategoryId == model.CategoryId &&
                            b.IsApproved &&
                            (b.Title.Contains(model.Input) ||
                            b.Publisher.Name.Contains(model.Input) ||
                            b.AuthorsBooks.Select(b => b.Author).Any(x => x.Name.Contains(model.Input))))
                            .CountAsync();
            }

            var books = await booksQuery.OrderByDescending(b => b.CreatedOn)
                        .Select(x => new BookViewModel { Id = x.Id, Title = x.Title, ImageUrl = x.ImageUrl })
                        .Skip((model.Page - 1) * BooksPerPage)
                        .Take(BooksPerPage)
                        .ToListAsync();

            return new BookListingViewModel
            {
                Books = books,
                RecordsCount = recordsCount,
                PageNumber = model.Page,
                ItemsPerPage = BooksPerPage,
            };
        }
    }
}
