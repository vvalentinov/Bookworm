namespace Bookworm.Services.Data.Models
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;

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

        public async Task<double> GetAverageRatingAsync(int bookId)
        {
            var book = await this.GetBookWithIdAsync(bookId);
            return book.Ratings.Count == 0 ? 0 : book.Ratings.Average(x => x.Value);
        }

        public async Task<int> GetUserRatingAsync(int bookId, string userId)
        {
            var book = await this.GetBookWithIdAsync(bookId);
            var rating = book.Ratings.FirstOrDefault(x => x.UserId == userId);
            return rating == null ? 0 : rating.Value;
        }

        public async Task<int> GetRatingsCountAsync(int bookId)
        {
            var book = await this.GetBookWithIdAsync(bookId);
            return book.Ratings.Count;
        }

        public async Task SetRatingAsync(int bookId, string userId, byte value)
        {
            var book = await this.GetBookWithIdAsync(bookId, withTracking: true);

            if (book.UserId == userId)
            {
                throw new InvalidOperationException(RateBookError);
            }

            var rating = book.Ratings.FirstOrDefault(r => r.UserId == userId);

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
        }

        private async Task<Book> GetBookWithIdAsync(int bookId, bool withTracking = false)
        {
            var query = withTracking ?
                this.bookRepository.All() :
                this.bookRepository.AllAsNoTracking();

            var book = await query
                .Where(b => b.IsApproved)
                .Include(x => x.Ratings)
                .FirstOrDefaultAsync(x => x.Id == bookId) ??
                throw new InvalidOperationException(BookWrongIdError);

            return book;
        }
    }
}
