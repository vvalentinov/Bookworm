namespace Bookworm.Services.Data.Contracts.Books
{
    using System.IO;
    using System.Threading.Tasks;

    using Bookworm.Common;
    using Bookworm.Data.Models;

    public interface IDownloadBookService
    {
        Task<OperationResult<(Stream stream, string contentType, string downloadName)>> DownloadBookAsync(
            int bookId,
            ApplicationUser user);
    }
}
