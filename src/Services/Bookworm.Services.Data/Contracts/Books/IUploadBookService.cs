namespace Bookworm.Services.Data.Contracts.Books
{
    using System.Threading.Tasks;

    using Bookworm.Common;
    using Bookworm.Web.ViewModels.DTOs;

    public interface IUploadBookService
    {
        Task<OperationResult> UploadBookAsync(
            BookDto bookDto,
            string userId);
    }
}
