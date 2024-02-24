namespace Bookworm.Services.Data.Contracts.Books
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Bookworm.Web.ViewModels.Books;

    public interface IFavoriteBooksService
    {
        IEnumerable<BookViewModel> GetUserFavoriteBooks(string userId);

        Task AddBookToFavoritesAsync(int bookId, string userId);

        Task DeleteFromFavoritesAsync(int bookId, string userId);
    }
}
