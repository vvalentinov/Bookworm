﻿namespace Bookworm.Services.Data.Contracts.Books
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Bookworm.Web.ViewModels.Authors;
    using Bookworm.Web.ViewModels.DTOs;

    public interface IUpdateBookService
    {
        Task DeleteBookAsync(string bookId, string userId);

        Task ApproveBookAsync(string bookId);

        Task UnapproveBookAsync(string bookId);

        Task UndeleteBookAsync(string bookId);

        Task EditBookAsync(
            BookDto editBookDto,
            IEnumerable<UploadAuthorViewModel> authors,
            string userId);
    }
}
