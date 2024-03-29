﻿namespace Bookworm.Services.Data.Models.Books
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Bookworm.Data.Common.Repositories;
    using Bookworm.Data.Models;
    using Bookworm.Services.Data.Contracts.Books;
    using Bookworm.Web.ViewModels.Books;

    public class FavoriteBookService : IFavoriteBooksService
    {
        private readonly IRepository<FavoriteBook> favoriteBooksRepository;
        private readonly IDeletableEntityRepository<Book> bookRepository;

        public FavoriteBookService(
            IRepository<FavoriteBook> favoriteBooksRepository,
            IDeletableEntityRepository<Book> bookRepository)
        {
            this.favoriteBooksRepository = favoriteBooksRepository;
            this.bookRepository = bookRepository;
        }

        public async Task AddBookToFavoritesAsync(int bookId, string userId)
        {
            var book = this.favoriteBooksRepository
                .AllAsNoTracking()
                .FirstOrDefault(x => x.UserId == userId && x.BookId == bookId);

            if (book != null)
            {
                throw new Exception("This book is already present in favorites!");
            }

            await this.favoriteBooksRepository.AddAsync(new FavoriteBook() { BookId = bookId, UserId = userId });
            await this.favoriteBooksRepository.SaveChangesAsync();
        }

        public async Task DeleteFromFavoritesAsync(int bookId, string userId)
        {
            FavoriteBook book = this.favoriteBooksRepository
                .All()
                .FirstOrDefault(x => x.UserId == userId && x.BookId == bookId);

            this.favoriteBooksRepository.Delete(book);
            await this.favoriteBooksRepository.SaveChangesAsync();
        }

        public IEnumerable<BookDetailsViewModel> GetUserFavoriteBooks(string userId)
        {
            List<int> bookIds = this.favoriteBooksRepository
                .AllAsNoTracking()
                .Where(x => x.UserId == userId)
                .Select(x => x.BookId)
                .ToList();

            return this.bookRepository
                .AllAsNoTracking()
                .Where(x => bookIds.Contains(x.Id))
                .Select(x => new BookDetailsViewModel
                {
                    Id = x.Id,
                    ImageUrl = x.ImageUrl,
                    Title = x.Title,
                }).ToList();
        }
    }
}
