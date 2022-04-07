namespace Bookworm.Services.Data.Models
{
    using System.Linq;
    using System.Threading.Tasks;

    using Bookworm.Data.Common.Repositories;
    using Bookworm.Data.Models;
    using Bookworm.Services.Data.Contracts;

    public class RatingsService : IRatingsService
    {
        private readonly IRepository<Rating> ratingRepository;

        public RatingsService(IRepository<Rating> ratingRepository)
        {
            this.ratingRepository = ratingRepository;
        }

        public double GetAverageVotes(string bookId)
        {
            return this.ratingRepository
                .AllAsNoTracking()
                .Where(x => x.BookId == bookId)
                .Average(x => x.Value);
        }

        public int? GetUserVote(string bookId, string userId)
        {
            return this.ratingRepository
                 .AllAsNoTracking()
                 .FirstOrDefault(x => x.BookId == bookId && x.UserId == userId)
                 .Value;
        }

        public int GetVotesCount(string bookId)
        {
            return this.ratingRepository
                .AllAsNoTracking()
                .Where(x => x.BookId == bookId)
                .Count();
        }

        public async Task SetVoteAsync(
            string bookId,
            string userId,
            byte value)
        {
            Rating rating = this.ratingRepository
                .All()
                .FirstOrDefault(x => x.BookId == bookId && x.UserId == userId);

            if (rating == null)
            {
                rating = new Rating()
                {
                    BookId = bookId,
                    UserId = userId,
                };

                await this.ratingRepository.AddAsync(rating);
            }

            rating.Value = value;
            await this.ratingRepository.SaveChangesAsync();
        }
    }
}
