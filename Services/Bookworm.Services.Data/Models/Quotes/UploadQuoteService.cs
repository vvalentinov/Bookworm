namespace Bookworm.Services.Data.Models.Quotes
{
    using System;
    using System.Threading.Tasks;

    using Bookworm.Data.Common.Repositories;
    using Bookworm.Data.Models;
    using Bookworm.Data.Models.Enums;
    using Bookworm.Services.Data.Contracts.Quotes;
    using Microsoft.EntityFrameworkCore;

    using static Bookworm.Common.Quotes.QuotesErrorMessagesConstants;

    public class UploadQuoteService : IUploadQuoteService
    {
        private readonly IDeletableEntityRepository<Quote> quoteRepository;
        private readonly IDeletableEntityRepository<ApplicationUser> userRepository;

        public UploadQuoteService(
            IDeletableEntityRepository<Quote> quoteRepository,
            IDeletableEntityRepository<ApplicationUser> userRepository)
        {
            this.quoteRepository = quoteRepository;
            this.userRepository = userRepository;
        }

        public async Task UploadBookQuoteAsync(
            string content,
            string bookTitle,
            string author,
            string userId)
        {
            await this.CheckIfQuoteAndUserExist(userId, content);

            var quote = new Quote
            {
                Content = content,
                BookTitle = bookTitle,
                AuthorName = author,
                UserId = userId,
                Type = QuoteType.BookQuote,
            };

            await this.quoteRepository.AddAsync(quote);
            await this.quoteRepository.SaveChangesAsync();
        }

        public async Task UploadGeneralQuoteAsync(
            string content,
            string authorName,
            string userId)
        {
            await this.CheckIfQuoteAndUserExist(userId, content);

            var quote = new Quote
            {
                Content = content,
                AuthorName = authorName,
                UserId = userId,
                Type = QuoteType.GeneralQuote,
            };

            await this.quoteRepository.AddAsync(quote);
            await this.quoteRepository.SaveChangesAsync();
        }

        public async Task UploadMovieQuoteAsync(
            string content,
            string movieTitle,
            string userId)
        {
            await this.CheckIfQuoteAndUserExist(userId, content);

            var quote = new Quote
            {
                Content = content,
                MovieTitle = movieTitle,
                UserId = userId,
                Type = QuoteType.MovieQuote,
            };

            await this.quoteRepository.AddAsync(quote);
            await this.quoteRepository.SaveChangesAsync();
        }

        private async Task CheckIfQuoteAndUserExist(string userId, string quoteContent)
        {
            bool userExists = await this.userRepository.AllAsNoTracking().AnyAsync(x => x.Id == userId);
            if (!userExists)
            {
                throw new InvalidOperationException("No username with given id found!");
            }

            bool quoteExist = await this.quoteRepository
                .AllAsNoTracking()
                .AnyAsync(x => x.Content.ToLower().Contains(quoteContent.Trim().ToLower()));

            if (quoteExist)
            {
                throw new InvalidOperationException(QuoteExistsError);
            }
        }
    }
}
