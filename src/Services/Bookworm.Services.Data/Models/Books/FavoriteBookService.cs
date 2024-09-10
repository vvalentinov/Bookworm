namespace Bookworm.Services.Data.Models.Books
{
    using System.Threading.Tasks;

    using Bookworm.Common;
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

        public async Task<OperationResult> AddBookToFavoritesAsync(
            int bookId,
            string userId)
        {
            var checkBookIdResult = await this.CheckBookIdAsync(bookId);

            if (checkBookIdResult.IsFailure)
            {
                return OperationResult.Fail(checkBookIdResult.ErrorMessage);
            }

            var bookIsFavorite = await this.favBookRepo
                .AllAsNoTracking()
                .AnyAsync(x => x.BookId == bookId && x.UserId == userId);

            if (bookIsFavorite)
            {
                return OperationResult.Fail(FavoriteBookIsAlreadyPresentError);
            }

            var favoriteBook = new FavoriteBook
            {
                BookId = bookId,
                UserId = userId,
            };

            await this.favBookRepo.AddAsync(favoriteBook);
            await this.favBookRepo.SaveChangesAsync();

            return OperationResult.Ok("Successfully added to favorites list!");
        }

        public async Task<OperationResult> DeleteBookFromFavoritesAsync(
            int bookId,
            string userId)
        {
            var checkBookIdResult = await this.CheckBookIdAsync(bookId);

            if (checkBookIdResult.IsFailure)
            {
                return OperationResult.Fail(checkBookIdResult.ErrorMessage);
            }

            var book = await this.favBookRepo
                .All()
                .FirstOrDefaultAsync(x => x.BookId == bookId && x.UserId == userId);

            if (book == null)
            {
                return OperationResult.Fail(FavoriteBookIsNotPresentError);
            }

            this.favBookRepo.Delete(book);
            await this.favBookRepo.SaveChangesAsync();

            return OperationResult.Ok("Successfully deleted from favorites list!");
        }

        private async Task<OperationResult> CheckBookIdAsync(int id)
        {
            var bookExists = await this.bookRepo
                .AllAsNoTracking()
                .AnyAsync(b => b.IsApproved && b.Id == id);

            return bookExists ?
                OperationResult.Ok() :
                OperationResult.Fail(BookWrongIdError);
        }
    }
}
