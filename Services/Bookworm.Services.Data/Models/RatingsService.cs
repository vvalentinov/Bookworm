﻿namespace Bookworm.Services.Data.Models
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

        public async Task<double> GetAverageRatingAsync(int bookId)
        {
            if (await this.GetRatingsCountAsync(bookId) > 0)
            {
                return await this.ratingRepository
                    .AllAsNoTracking()
                    .Where(x => x.BookId == bookId)
                    .AverageAsync(x => x.Value);
            }

            return 0;
        }

        public async Task<int> GetUserRatingAsync(int bookId, string userId)
        {
            var rating = await this.ratingRepository
                 .AllAsNoTracking()
                 .FirstOrDefaultAsync(x => x.BookId == bookId && x.UserId == userId);

            return rating == null ? 0 : rating.Value;
        }

        public async Task<int> GetRatingsCountAsync(int bookId)
            => await this.ratingRepository
                .AllAsNoTracking()
                .Where(x => x.BookId == bookId)
                .CountAsync();

        public async Task SetRatingAsync(int bookId, string userId, byte value)
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
