namespace Bookworm.Services.Data
{
    using System.Linq;

    using Bookworm.Data.Models;
    using Bookworm.Web.ViewModels.Books;
    using Bookworm.Web.ViewModels.Quotes;
    using Microsoft.EntityFrameworkCore;

    public static class QueryableExtensions
    {
        public static IQueryable<QuoteViewModel> ToQuoteViewModel(
            this IQueryable<Quote> quotes,
            string userId = null)
        {
            return quotes.Select(quote => new QuoteViewModel
            {
                Id = quote.Id,
                AuthorName = quote.AuthorName,
                BookTitle = quote.BookTitle,
                Content = quote.Content,
                CreatedOn = quote.CreatedOn,
                IsApproved = quote.IsApproved,
                MovieTitle = quote.MovieTitle,
                Type = quote.Type,
                UserId = quote.UserId,
                Likes = quote.Likes,
                IsUserQuoteCreator = userId != null && quote.UserId == userId,
            });
        }

        public static IQueryable<BookViewModel> SelectBookViewModel(this IQueryable<Book> books)
        {
            return books.Select(x => new BookViewModel
            {
                Id = x.Id,
                Title = x.Title,
                ImageUrl = x.ImageUrl,
            });
        }

        public static IQueryable<BookDetailsViewModel> SelectBookDetailsViewModel(this IQueryable<Book> book)
        {
            return book.Select(x => new BookDetailsViewModel
            {
                Id = x.Id,
                Title = x.Title,
                UserId = x.UserId,
            });
        }

        public static IQueryable<Book> FilterUserBooksBasedOnSearch(
            this IQueryable<Book> book,
            string search,
            string userId)
        {
            search = $"%{search}%";

            return book.Where(b => b.UserId == userId &&
                        (EF.Functions.Like(b.Title, search) ||
                        EF.Functions.Like(b.Publisher.Name, search) ||
                        b.AuthorsBooks.Any(ab => EF.Functions.Like(ab.Author.Name, search))));
        }

        public static IQueryable<Book> FilterBooksInCategoryBasedOnSearch(
            this IQueryable<Book> book, string search, int categoryId)
        {
            search = $"%{search}%";

            return book.Where(b => b.IsApproved && b.CategoryId == categoryId &&
                            (EF.Functions.Like(b.Title, search) ||
                            EF.Functions.Like(b.Publisher.Name, search) ||
                            b.AuthorsBooks.Any(ab => EF.Functions.Like(ab.Author.Name, search))));
        }
    }
}
