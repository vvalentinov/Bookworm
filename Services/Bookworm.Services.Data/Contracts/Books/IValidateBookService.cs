namespace Bookworm.Services.Data.Contracts.Books
{
    using System.Threading.Tasks;

    public interface IValidateBookService
    {
        Task ValidateAsync(
            string title,
            int languageId,
            int categoryId,
            int? bookId = null);
    }
}
