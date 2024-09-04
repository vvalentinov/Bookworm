namespace Bookworm.Services.Data.Contracts.Books
{
    using System.Threading.Tasks;

    using Bookworm.Common;

    public interface IFavoriteBookService
    {
        Task<OperationResult> AddBookToFavoritesAsync(int bookId, string userId);

        Task<OperationResult> DeleteBookFromFavoritesAsync(int bookId, string userId);
    }
}
