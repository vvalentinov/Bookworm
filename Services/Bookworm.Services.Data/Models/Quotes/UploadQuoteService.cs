namespace Bookworm.Services.Data.Models.Quotes
{
    using System;
    using System.Threading.Tasks;

    using Bookworm.Common.Enums;
    using Bookworm.Data.Common.Repositories;
    using Bookworm.Data.Models;
    using Bookworm.Services.Data.Contracts.Quotes;
    using Bookworm.Web.ViewModels.DTOs;
    using Microsoft.EntityFrameworkCore;

    using static Bookworm.Common.Constants.ErrorMessagesConstants.QuoteErrorMessagesConstants;

    public class UploadQuoteService : IUploadQuoteService
    {
        private readonly IDeletableEntityRepository<Quote> quoteRepository;

        public UploadQuoteService(IDeletableEntityRepository<Quote> quoteRepository)
        {
            this.quoteRepository = quoteRepository;
        }

        public async Task UploadQuoteAsync(QuoteDto quoteDto, string userId)
        {
            string content = quoteDto.Content.Trim();

            bool quoteExist = await this.quoteRepository
                .AllAsNoTracking()
                .AnyAsync(x => x.Content.ToLower() == content.ToLower());

            if (quoteExist)
            {
                throw new InvalidOperationException(QuoteExistsError);
            }

            var quote = new Quote { Content = content, UserId = userId };

            switch (quoteDto.Type)
            {
                case QuoteType.BookQuote:
                    quote.AuthorName = quoteDto.AuthorName.Trim();
                    quote.BookTitle = quoteDto.BookTitle.Trim();
                    quote.Type = QuoteType.BookQuote;
                    break;
                case QuoteType.MovieQuote:
                    quote.MovieTitle = quoteDto.MovieTitle.Trim();
                    quote.Type = QuoteType.MovieQuote;
                    break;
                case QuoteType.GeneralQuote:
                    quote.AuthorName = quoteDto.AuthorName.Trim();
                    quote.Type = QuoteType.GeneralQuote;
                    break;
                default: throw new InvalidOperationException(QuoteInvalidTypeError);
            }

            await this.quoteRepository.AddAsync(quote);
            await this.quoteRepository.SaveChangesAsync();
        }
    }
}
