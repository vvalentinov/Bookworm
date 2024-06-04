namespace Bookworm.Services.Data
{
    using System.Linq;

    using Bookworm.Data.Models;
    using Bookworm.Web.ViewModels.Books;
    using Bookworm.Web.ViewModels.Comments;

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
            return book.Where(x => x.UserId == userId &&
                            (x.Title.Contains(search) ||
                            x.Publisher.Name.Contains(search) ||
                            x.AuthorsBooks.Select(b => b.Author).Any(x => x.Name.Contains(search))));
        }

        public static IQueryable<Book> FilterBooksInCategoryBasedOnSearch(
            this IQueryable<Book> book,
            string search,
            int categoryId)
        {
            return book.Where(b => b.IsApproved && b.CategoryId == categoryId &&
                            (b.Title.Contains(search) ||
                            b.Publisher.Name.Contains(search) ||
                            b.AuthorsBooks.Select(b => b.Author).Any(x => x.Name.Contains(search))));
        }
    }
}
