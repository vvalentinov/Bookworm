namespace Bookworm.Services.Data.Contracts.Books
{
    using System;
    using System.IO;
    using System.Threading.Tasks;

    public interface IDownloadBookService
    {
        Task<Tuple<Stream, string, string>> DownloadBookAsync(int bookId);
    }
}
