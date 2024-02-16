namespace Bookworm.Services.Data.Models
{
    using System.Linq;
    using System.Threading.Tasks;

    using Bookworm.Data.Common.Repositories;
    using Bookworm.Data.Models;
    using Bookworm.Services.Data.Contracts;
    using Microsoft.EntityFrameworkCore;

    public class RatingsService : IRatingsService
    {
        private readonly IRepository<Rating> ratingRepository;

        public RatingsService(IRepository<Rating> ratingRepository)
        {
            this.ratingRepository = ratingRepository;
        }

        public async Task<double> GetAverageRatingAsync(string bookId)
            => await this.ratingRepository
                    .AllAsNoTracking()
                    .Where(x => x.BookId == bookId)
                    .AverageAsync(x => x.Value);

        public async Task<int> GetUserRatingAsync(string bookId, string userId)
        {
            var rating = await this.ratingRepository
                 .AllAsNoTracking()
                 .FirstOrDefaultAsync(x => x.BookId == bookId && x.UserId == userId);

            return rating == null ? 0 : rating.Value;
        }

        public async Task<int> GetRatingsCountAsync(string bookId)
        {
            return await this.ratingRepository
                .AllAsNoTracking()
                .Where(x => x.BookId == bookId)
                .CountAsync();
        }

        public async Task SetRatingAsync(string bookId, string userId, byte value)
        {
            Rating rating = await this.ratingRepository
                .All()
                .FirstOrDefaultAsync(x => x.BookId == bookId && x.UserId == userId);

            if (rating == null)
            {
                rating = new Rating()
                {
                    BookId = bookId,
                    UserId = userId,
                    Value = value,
                };

                await this.ratingRepository.AddAsync(rating);
            }
            else
            {
                rating.Value = value;
                this.ratingRepository.Update(rating);
            }

            await this.ratingRepository.SaveChangesAsync();
        }
    }
}
