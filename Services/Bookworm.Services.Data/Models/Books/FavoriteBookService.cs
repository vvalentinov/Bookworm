namespace Bookworm.Services.Data.Models.Books
{
    using System;
    using System.Threading.Tasks;

    using Bookworm.Data.Common.Repositories;
    using Bookworm.Data.Models;
    using Bookworm.Services.Data.Contracts.Books;
    using Microsoft.EntityFrameworkCore;

    using static Bookworm.Common.Constants.ErrorMessagesConstants.BookErrorMessagesConstants;
    using static Bookworm.Common.Constants.ErrorMessagesConstants.FavoriteBookMessagesConstants;

    public class FavoriteBookService : IFavoriteBookService
    {
        private readonly IRepository<FavoriteBook> favBookRepo;
        private readonly IDeletableEntityRepository<Book> bookRepo;

        public FavoriteBookService(
            IRepository<FavoriteBook> favBookRepo,
            IDeletableEntityRepository<Book> bookRepo)
        {
            this.bookRepo = bookRepo;
            this.favBookRepo = favBookRepo;
        }

        public async Task AddBookToFavoritesAsync(int bookId, string userId)
        {
            await this.CheckBookIdAsync(bookId);

            if (await this.favBookRepo.AllAsNoTracking().AnyAsync(x => x.BookId == bookId && x.UserId == userId))
            {
                throw new InvalidOperationException(FavoriteBookIsAlreadyPresentError);
            }

            await this.favBookRepo.AddAsync(new FavoriteBook { BookId = bookId, UserId = userId });
            await this.favBookRepo.SaveChangesAsync();
        }

        public async Task DeleteBookFromFavoritesAsync(int bookId, string userId)
        {
            await this.CheckBookIdAsync(bookId);

            var favBook = await this.favBookRepo
                .All()
                .FirstOrDefaultAsync(x => x.BookId == bookId && x.UserId == userId) ??
                throw new InvalidOperationException(FavoriteBookIsNotPresentError);

            this.favBookRepo.Delete(favBook);
            await this.favBookRepo.SaveChangesAsync();
        }

        private async Task CheckBookIdAsync(int id)
        {
            if (!await this.bookRepo.AllAsNoTracking().AnyAsync(b => b.Id == id && b.IsApproved))
            {
                throw new InvalidOperationException(BookWrongIdError);
            }
        }
    }
}
