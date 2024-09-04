namespace Bookworm.Services.Data.Models.Books
{
    using System.Linq;
    using System.Threading.Tasks;

    using Bookworm.Common;
    using Bookworm.Data.Common.Repositories;
    using Bookworm.Data.Models;
    using Bookworm.Services.Data.Contracts.Books;
    using Bookworm.Web.ViewModels.Books;
    using Microsoft.EntityFrameworkCore;

    using static Bookworm.Common.Constants.DataConstants.BookDataConstants;

    public class SearchBooksService : ISearchBooksService
    {
        private readonly IDeletableEntityRepository<Book> bookRepository;

        public SearchBooksService(IDeletableEntityRepository<Book> bookRepository)
        {
            this.bookRepository = bookRepository;
        }

        public async Task<OperationResult<bool>> CheckIfBookWithTitleExistsAsync(
            string title,
            int? bookId = null)
        {
            var query = this.bookRepository
                .AllAsNoTrackingWithDeleted()
                .Where(b => b.Title == title);

            if (bookId.HasValue)
            {
                query = query.Where(b => b.Id != bookId.Value);
            }

            var bookExists = await query.AnyAsync();

            return OperationResult.Ok(bookExists);
        }

        public async Task<OperationResult<BookListingViewModel>> SearchBooksAsync(SearchBookInputModel model)
        {
            var query = (IQueryable<Book>)this.bookRepository
                .AllAsNoTracking()
                .Include(b => b.Publisher)
                .Include(b => b.AuthorsBooks)
                .ThenInclude(b => b.Author);

            if (model.LanguagesIds.Count > 0)
            {
                query = query.Where(b => model.LanguagesIds.Contains(b.LanguageId));
            }

            query = model.IsForUserBooks ?
                query.FilterUserBooksBasedOnSearch(model.Input.Trim(), model.UserId) :
                query.FilterBooksInCategoryBasedOnSearch(model.Input.Trim(), model.CategoryId);

            int recordsCount = await query.CountAsync();

            var books = await query
                        .OrderByDescending(b => b.CreatedOn)
                        .SelectBookViewModel()
                        .Skip((model.Page - 1) * BooksPerPage)
                        .Take(BooksPerPage)
                        .ToListAsync();

            var bookListingModel = new BookListingViewModel
            {
                Books = books,
                PageNumber = model.Page,
                RecordsCount = recordsCount,
                ItemsPerPage = BooksPerPage,
            };

            return OperationResult.Ok(bookListingModel);
        }
    }
}
