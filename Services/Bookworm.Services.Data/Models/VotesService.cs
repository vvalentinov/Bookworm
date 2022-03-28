﻿namespace Bookworm.Services.Data.Models
{
    using System.Linq;
    using System.Threading.Tasks;

    using Bookworm.Data.Common.Repositories;
    using Bookworm.Data.Models;
    using Bookworm.Services.Data.Contracts;
    using Microsoft.EntityFrameworkCore;

    public class VotesService : IVotesService
    {
        private readonly IRepository<Rating> votesRepository;

        public VotesService(IRepository<Rating> votesRepository)
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

        public int GetVotesCount(string bookId)
        {
            return this.votesRepository.AllAsNoTracking().Where(x => x.BookId == bookId).Count();
        }

        public async Task SetVoteAsync(
            string bookId,
            string userId,
            byte value)
        {
            Rating vote = this.votesRepository
                .All()
                .FirstOrDefault(x => x.BookId == bookId && x.UserId == userId);

            if (vote == null)
            {
                vote = new Rating()
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
