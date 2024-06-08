namespace Bookworm.Services.Data
{
    using System.Linq;

    using Bookworm.Data.Models;
    using Bookworm.Web.ViewModels.Books;
    using Bookworm.Web.ViewModels.Comments;
    using Microsoft.EntityFrameworkCore;

    public static class QueryableExtensions
    {
        public static IQueryable<CommentViewModel> SelectComments(
            this IQueryable<Comment> comments, string userId)
        {
            return comments.Select(c => new CommentViewModel
            {
                Id = c.Id,
                UserId = c.UserId,
                Content = c.Content,
                NetWorth = c.NetWorth,
                CreatedOn = c.CreatedOn,
                UserUserName = c.User.UserName,
                IsCommentOwner = c.UserId == userId,
                UserVoteValue = c.Votes
                            .Where(v => v.CommentId == c.Id && v.UserId == userId)
                            .Select(v => (int)v.Value)
                            .FirstOrDefault(),
            });
        }

        public static IQueryable<BookViewModel> SelectBookViewModel(this IQueryable<Book> book)
        {
            return book.Select(x => new BookViewModel { Id = x.Id, Title = x.Title, ImageUrl = x.ImageUrl });
        }

        public static IQueryable<BookDetailsViewModel> SelectBookDetailsViewModel(this IQueryable<Book> book)
        {
            return book.Select(x => new BookDetailsViewModel { Id = x.Id, Title = x.Title, UserId = x.UserId });
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
