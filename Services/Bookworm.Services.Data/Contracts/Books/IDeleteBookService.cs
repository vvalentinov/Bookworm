namespace Bookworm.Services.Data.Contracts.Books
{
    using System.Threading.Tasks;

    public interface IDeleteBookService
    {
        Task DeleteBookAsync(string bookId);
    }
}
