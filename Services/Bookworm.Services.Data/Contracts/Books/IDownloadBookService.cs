namespace Bookworm.Services.Data.Contracts.Books
{
    using System;
    using System.IO;
    using System.Threading.Tasks;

    using Bookworm.Data.Models;

    public interface IDownloadBookService
    {
        Task<Tuple<Stream, string, string>> DownloadBookAsync(
            int bookId,
            ApplicationUser user,
            bool isCurrUserAdmin = false);
    }
}
