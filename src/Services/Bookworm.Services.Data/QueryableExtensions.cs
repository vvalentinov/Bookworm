namespace Bookworm.Services.Data
{
    using System.Linq;

    using Bookworm.Data.Models;
    using Bookworm.Web.ViewModels.Books;
    using Bookworm.Web.ViewModels.Categories;
    using Bookworm.Web.ViewModels.Comments;
    using Bookworm.Web.ViewModels.Languages;
    using Bookworm.Web.ViewModels.Notification;
    using Bookworm.Web.ViewModels.Quotes;
    using Bookworm.Web.ViewModels.Settings;
    using Microsoft.EntityFrameworkCore;

    public static class QueryableExtensions
    {
        public static IQueryable<BookDetailsViewModel> ToBookDetailsViewModel(
            this IQueryable<Book> books,
            string userId)
        {
            return books.Select(book => new BookDetailsViewModel
            {
                Authors = book
                    .AuthorsBooks
                    .Select(x => x.Author.Name)
                    .ToList(),
                CategoryName = book.Category.Name,
                Description = book.Description,
                DownloadsCount = book.DownloadsCount,
                FileUrl = book.FileUrl,
                Id = book.Id,
                ImageUrl = book.ImageUrl,
                IsApproved = book.IsApproved,
                Language = book.Language.Name,
                PagesCount = book.PagesCount,
                PublisherName = book.Publisher.Name,
                Title = book.Title,
                UserId = book.UserId,
                Username = book.User.UserName,
                Year = book.Year,
                IsUserBook = book.UserId == userId,
            });
        }

        public static IQueryable<CommentViewModel> ToCommentViewModel(
            this IQueryable<Comment> comments,
            string userId)
        {
            return comments.Select(comment => new CommentViewModel
            {
                Id = comment.Id,
                Content = comment.Content,
                CreatedOn = comment.CreatedOn,
                NetWorth = comment.NetWorth,
                UserId = comment.UserId,
                UserUserName = comment.User.UserName,
                Votes = comment.Votes,
                IsCommentOwner = userId != null && comment.UserId == userId,
                UserVoteValue = userId == null ? 0 : comment.Votes
                    .Where(v => v.UserId == userId && v.CommentId == comment.Id)
                    .Select(v => (int?)v.Value)
                    .FirstOrDefault() ?? 0,
            });
        }

        public static IQueryable<LanguageViewModel> ToLanguageViewModel(this IQueryable<Language> languages)
        {
            return languages.Select(language => new LanguageViewModel
            {
                Id = language.Id,
                Name = language.Name,
            });
        }

        public static IQueryable<NotificationViewModel> ToNotificationViewModel(this IQueryable<Notification> notifications)
        {
            return notifications.Select(notification => new NotificationViewModel
            {
                Id = notification.Id,
                Content = notification.Content,
            });
        }

        public static IQueryable<CategoryViewModel> ToCategoryViewModel(this IQueryable<Category> categories)
        {
            return categories.Select(category => new CategoryViewModel
            {
                Id = category.Id,
                Name = category.Name,
            });
        }

        public static IQueryable<SettingViewModel> ToSettingViewModel(this IQueryable<Setting> settings)
        {
            return settings.Select(setting => new SettingViewModel
            {
                Id = setting.Id,
                Name = setting.Name,
                Value = setting.Value,
                NameAndValue = $"{setting.Name} = {setting.Value}",
            });
        }

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

        public static IQueryable<BookViewModel> SelectBookViewModel(this IQueryable<Book> books)
        {
            return books.Select(x => new BookViewModel
            {
                Id = x.Id,
                Title = x.Title,
                ImageUrl = x.ImageUrl,
            });
        }

        public static IQueryable<BookViewModel> SelectBookViewModel(this IQueryable<FavoriteBook> favBooks)
        {
            return favBooks.Select(x => new BookViewModel
            {
                Id = x.Book.Id,
                Title = x.Book.Title,
                ImageUrl = x.Book.ImageUrl,
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
