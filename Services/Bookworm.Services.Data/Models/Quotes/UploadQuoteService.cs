namespace Bookworm.Services.Data.Models.Quotes
{
    using System;
    using System.Threading.Tasks;

    using Bookworm.Common.Enums;
    using Bookworm.Data.Common.Repositories;
    using Bookworm.Data.Models;
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

        public async Task UploadQuoteAsync(QuoteDto quoteDto, string userId)
        {
            bool userExists = await this.userRepository.AllAsNoTracking().AnyAsync(x => x.Id == userId);
            if (!userExists)
            {
                throw new InvalidOperationException("No username with given id found!");
            }

            bool quoteExist = await this.quoteRepository
                .AllAsNoTracking()
                .AnyAsync(x => x.Content.ToLower() == quoteDto.Content.Trim().ToLower());
            if (quoteExist)
            {
                throw new InvalidOperationException(QuoteExistsError);
            }

            var quote = new Quote { Content = quoteDto.Content.Trim() };

            switch (quoteDto.Type)
            {
                case QuoteType.BookQuote:
                    quote.AuthorName = quoteDto.AuthorName.Trim();
                    quote.BookTitle = quoteDto.BookTitle.Trim();
                    break;
                case QuoteType.MovieQuote:
                    quote.MovieTitle = quoteDto.MovieTitle.Trim();
                    break;
                case QuoteType.GeneralQuote:
                    quote.AuthorName = quoteDto.AuthorName.Trim();
                    break;
            }

            await this.quoteRepository.AddAsync(quote);
            await this.quoteRepository.SaveChangesAsync();
        }
    }
}
