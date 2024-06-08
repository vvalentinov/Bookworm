namespace Bookworm.Services.Data.Contracts.Books
{
    using System.Threading.Tasks;

    public interface IFavoriteBookService
    {
        Task AddBookToFavoritesAsync(int bookId, string userId);

        Task DeleteBookFromFavoritesAsync(int bookId, string userId);
    }
}
