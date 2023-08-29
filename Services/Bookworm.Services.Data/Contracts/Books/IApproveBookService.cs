namespace Bookworm.Services.Data.Contracts.Books
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public interface IApproveBookService
    {
        Task ApproveBook(string bookId, string userId);
    }
}
