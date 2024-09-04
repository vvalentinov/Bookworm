namespace Bookworm.Services.Data.Contracts.Books
{
    using System.Threading.Tasks;

    using Bookworm.Common;

    public interface IValidateBookService
    {
        Task<OperationResult> ValidateAsync(
            string title,
            int languageId,
            int categoryId,
            int? bookId = null);
    }
}
