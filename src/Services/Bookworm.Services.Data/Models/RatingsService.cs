namespace Bookworm.Services.Data.Models
{
    using System.Linq;
    using System.Threading.Tasks;

    using Bookworm.Common;
    using Bookworm.Data.Common.Repositories;
    using Bookworm.Data.Models;
    using Bookworm.Services.Data.Contracts;
    using Microsoft.EntityFrameworkCore;

    using static Bookworm.Common.Constants.ErrorMessagesConstants.BookErrorMessagesConstants;
    using static Bookworm.Common.Constants.ErrorMessagesConstants.RatingErrorMessagesConstants;

    public class RatingsService : IRatingsService
    {
        private readonly IDeletableEntityRepository<Book> bookRepository;

        public RatingsService(IDeletableEntityRepository<Book> bookRepository)
        {
            this.bookRepository = bookRepository;
        }

        public async Task<OperationResult<double>> GetAverageRatingAsync(int bookId)
        {
            var result = await this.GetBookWithIdAsync(bookId);

            if (result.IsFailure)
            {
                return OperationResult.Fail<double>(result.ErrorMessage);
            }

            var book = result.Data;

            if (book.Ratings.Count == 0)
            {
                return OperationResult.Ok<double>(0);
            }

            var ratingsAvg = book
                .Ratings
                .Average(x => x.Value);

            return OperationResult.Ok(ratingsAvg);
        }

        public async Task<OperationResult<int>> GetUserRatingAsync(
            int bookId,
            string userId)
        {
            var result = await this.GetBookWithIdAsync(bookId);

            if (result.IsFailure)
            {
                return OperationResult.Fail<int>(result.ErrorMessage);
            }

            var book = result.Data;

            var rating = book
                .Ratings
                .FirstOrDefault(x => x.UserId == userId);

            return rating == null ?
                OperationResult.Ok(0) :
                OperationResult.Ok<int>(rating.Value);
        }

        public async Task<OperationResult<int>> GetRatingsCountAsync(int bookId)
        {
            var result = await this.GetBookWithIdAsync(bookId);

            if (result.IsFailure)
            {
                return OperationResult.Fail<int>(result.ErrorMessage);
            }

            var book = result.Data;

            var ratingsCount = book.Ratings.Count;

            return OperationResult.Ok(ratingsCount);
        }

        public async Task<OperationResult> SetRatingAsync(
            int bookId,
            string userId,
            byte value)
        {
            var result = await this.GetBookWithIdAsync(
                bookId,
                withTracking: true);

            if (result.IsFailure)
            {
                return OperationResult.Fail(result.ErrorMessage);
            }

            var book = result.Data;

            if (book.UserId == userId)
            {
                return OperationResult.Fail(RateBookError);
            }

            var rating = book
                .Ratings
                .FirstOrDefault(r => r.UserId == userId);

            if (rating == null)
            {
                book.Ratings.Add(new Rating
                {
                    Value = value,
                    BookId = bookId,
                    UserId = userId,
                });
            }
            else if (rating.Value != value)
            {
                rating.Value = value;
                this.bookRepository.Update(book);
            }

            await this.bookRepository.SaveChangesAsync();

            return OperationResult.Ok();
        }

        private async Task<OperationResult<Book>> GetBookWithIdAsync(
            int bookId,
            bool withTracking = false)
        {
            var query = withTracking ?
                this.bookRepository.All() :
                this.bookRepository.AllAsNoTracking();

            var book = await query
                .Where(b => b.IsApproved)
                .Include(x => x.Ratings)
                .FirstOrDefaultAsync(x => x.Id == bookId);

            if (book == null)
            {
                return OperationResult.Fail<Book>(BookWrongIdError);
            }

            return OperationResult.Ok(book);
        }
    }
}
