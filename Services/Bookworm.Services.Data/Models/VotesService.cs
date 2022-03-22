namespace Bookworm.Services.Data.Models
{
    using System.Linq;
    using System.Threading.Tasks;

    using Bookworm.Data.Common.Repositories;
    using Bookworm.Data.Models;
    using Bookworm.Services.Data.Contracts;
    using Microsoft.EntityFrameworkCore;

    public class VotesService : IVotesService
    {
        private readonly IRepository<Vote> votesRepository;

        public VotesService(IRepository<Vote> votesRepository)
        {
            this.votesRepository = votesRepository;
        }

        public async Task<double> GetAverageVotesAsync(string bookId)
        {
            return await this.votesRepository
                .All()
                .Where(x => x.BookId == bookId)
                .AverageAsync(x => x.Value);
        }

        public int? GetUserVote(string bookId, string userId)
        {
            return this.votesRepository.All().FirstOrDefault(x => x.BookId == bookId && x.UserId == userId).Value;
        }

        public async Task SetVoteAsync(
            string bookId,
            string userId,
            byte value)
        {
            Vote vote = this.votesRepository
                .All()
                .FirstOrDefault(x => x.BookId == bookId && x.UserId == userId);

            if (vote == null)
            {
                vote = new Vote()
                {
                    BookId = bookId,
                    UserId = userId,
                };

                await this.votesRepository.AddAsync(vote);
            }

            vote.Value = value;
            await this.votesRepository.SaveChangesAsync();
        }
    }
}
