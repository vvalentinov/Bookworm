﻿namespace Bookworm.Services.Data.Contracts
{
    using System.Threading.Tasks;

    public interface IDeleteBookService
    {
        Task DeleteBookAsync(string bookId);
    }
}
