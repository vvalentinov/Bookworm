namespace Bookworm.Services.Data.Models.Books
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

        public FavoriteBookService(IRepository<FavoriteBook> favoriteBooksRepository, IDeletableEntityRepository<Book> bookRepository)
        {
            this.favoriteBooksRepository = favoriteBooksRepository;
            this.bookRepository = bookRepository;
        }

        public async Task AddBookToFavoritesAsync(string bookId, string userId)
        {
            var book = favoriteBooksRepository
                .AllAsNoTracking()
                .FirstOrDefault(x => x.UserId == userId && x.BookId == bookId);

            if (book != null)
            {
                throw new Exception("This book is already present in favorites!");
            }

            await favoriteBooksRepository.AddAsync(new FavoriteBook() { BookId = bookId, UserId = userId });
            await favoriteBooksRepository.SaveChangesAsync();
        }

        public async Task DeleteFromFavoritesAsync(string bookId, string userId)
        {
            FavoriteBook book = favoriteBooksRepository
                .All()
                .FirstOrDefault(x => x.UserId == userId && x.BookId == bookId);

            favoriteBooksRepository.Delete(book);
            await favoriteBooksRepository.SaveChangesAsync();
        }

        public IEnumerable<BookViewModel> GetUserFavoriteBooks(string userId)
        {
            List<string> bookIds = favoriteBooksRepository
                .AllAsNoTracking()
                .Where(x => x.UserId == userId)
                .Select(x => x.BookId)
                .ToList();

            return bookRepository
                .AllAsNoTracking()
                .Where(x => bookIds.Contains(x.Id))
                .Select(x => new BookViewModel()
                {
                    Id = x.Id,
                    ImageUrl = x.ImageUrl,
                    Title = x.Title,
                }).ToList();
        }
    }
}
