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

        public RatingsService(IRepository<Rating> votesRepository)
        {
            this.ratingRepository = votesRepository;
        }

        public async Task<double> GetAverageVotesAsync(string bookId)
        {
            return await this.ratingRepository
                .All()
                .Where(x => x.BookId == bookId)
                .AverageAsync(x => x.Value);
        }

        public int? GetUserVote(string bookId, string userId)
        {
            return this.ratingRepository.All().FirstOrDefault(x => x.BookId == bookId && x.UserId == userId).Value;
        }

        public int GetVotesCount(string bookId)
        {
            return this.ratingRepository.AllAsNoTracking().Where(x => x.BookId == bookId).Count();
        }

        public async Task SetVoteAsync(
            string bookId,
            string userId,
            byte value)
        {
            Rating vote = this.ratingRepository
                .All()
                .FirstOrDefault(x => x.BookId == bookId && x.UserId == userId);

            if (vote == null)
            {
                vote = new Rating()
                {
                    BookId = bookId,
                    UserId = userId,
                };

                await this.ratingRepository.AddAsync(vote);
            }

            vote.Value = value;
            await this.ratingRepository.SaveChangesAsync();
        }
    }
}
