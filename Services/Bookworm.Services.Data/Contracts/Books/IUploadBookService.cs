namespace Bookworm.Services.Data.Contracts.Books
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Bookworm.Data.Models;
    using Bookworm.Web.ViewModels.Authors;

    public interface IUploadBookService
    {
        Task UploadBookAsync(
            BookDto uploadBookDto,
            ICollection<UploadAuthorViewModel> authors,
            string userId);
    }
}
