﻿namespace Bookworm.Services.Data.Models.Quotes
{
    using System.Linq;
    using System.Threading.Tasks;

    using Bookworm.Data.Common.Repositories;
    using Bookworm.Data.Models;
    using Bookworm.Services.Data.Contracts.Quotes;
    using Microsoft.EntityFrameworkCore;

    public class UpdateQuoteService : IUpdateQuoteService
    {
        private readonly IDeletableEntityRepository<Quote> quoteRepository;
        private readonly IDeletableEntityRepository<UserPoints> userPointsRepository;

        public UpdateQuoteService(
            IDeletableEntityRepository<Quote> quoteRepository,
            IDeletableEntityRepository<UserPoints> userPointsRepository)
        {
            this.quoteRepository = quoteRepository;
            this.userPointsRepository = userPointsRepository;
        }

        public async Task ApproveQuoteAsync(int quoteId, string userId)
        {
            Quote quote = this.quoteRepository.All().First(x => x.Id == quoteId);
            quote.IsApproved = true;
            await this.quoteRepository.SaveChangesAsync();

            UserPoints userPoints = await this.userPointsRepository.All().FirstOrDefaultAsync(x => x.UserId == userId);
            if (userPoints == null)
            {
                userPoints = new UserPoints()
                {
                    UserId = userId,
                    Points = 2,
                };

                await this.userPointsRepository.AddAsync(userPoints);
            }
            else
            {
                userPoints.Points += 2;
            }

            await this.userPointsRepository.SaveChangesAsync();
        }

        public async Task DeleteQuoteAsync(int quoteId)
        {
            Quote quote = this.quoteRepository.All().First(x => x.Id == quoteId);
            this.quoteRepository.Delete(quote);
            quote.IsApproved = false;
            await this.quoteRepository.SaveChangesAsync();
        }

        public async Task UndeleteQuoteAsync(int quoteId)
        {
            Quote quote = this.quoteRepository.AllWithDeleted().First(x => x.Id == quoteId);
            this.quoteRepository.Undelete(quote);
            await this.quoteRepository.SaveChangesAsync();
        }

        public async Task UnapproveQuoteAsync(int quoteId)
        {
            Quote quote = this.quoteRepository.All().First(x => x.Id == quoteId);
            quote.IsApproved = false;
            await this.quoteRepository.SaveChangesAsync();

            UserPoints userPoints = this.userPointsRepository.All().First(x => x.UserId == quote.UserId);
            if (userPoints.Points > 0)
            {
                userPoints.Points -= 2;
            }

            await this.userPointsRepository.SaveChangesAsync();
        }

        public async Task EditQuoteAsync(
            int quoteId,
            string content,
            string authorName,
            string bookTitle,
            string movieTitle)
        {
            Quote quote = this.quoteRepository.All().First(x => x.Id == quoteId);
            quote.Content = content;
            quote.AuthorName = authorName;
            quote.BookTitle = bookTitle;
            quote.MovieTitle = movieTitle;

            this.quoteRepository.Update(quote);
            await this.quoteRepository.SaveChangesAsync();
        }
    }
}