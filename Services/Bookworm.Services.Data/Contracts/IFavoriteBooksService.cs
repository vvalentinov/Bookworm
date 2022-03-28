namespace Bookworm.Services.Data.Contracts
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Bookworm.Web.ViewModels.Books;

    public interface IFavoriteBooksService
    {
        IEnumerable<BookViewModel> GetUserFavoriteBooks(string userId);

        Task AddBookToFavoritesAsync(string bookId, string userId);

        Task DeleteFromFavoritesAsync(string bookId, string userId);
    }
}
