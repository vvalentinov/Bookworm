namespace Bookworm.Services.Data.Contracts.Books
{
    using System.Threading.Tasks;

    using Bookworm.Web.ViewModels.DTOs;

    public interface IUploadBookService
    {
        Task UploadBookAsync(BookDto uploadBookDto);
    }
}
