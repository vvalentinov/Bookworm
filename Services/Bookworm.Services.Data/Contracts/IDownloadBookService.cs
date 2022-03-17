namespace Bookworm.Services.Data.Contracts
{
    using System;
    using System.IO;
    using System.Threading.Tasks;

    public interface IDownloadBookService
    {
        Task<Tuple<Stream, string, string>> DownloadAsync(string bookId);
    }
}
