namespace Bookworm.Services.Data.Models.Quotes
{
    using System.Threading.Tasks;

    using Bookworm.Common;
    using Bookworm.Data.Common.Repositories;
    using Bookworm.Data.Models;
    using Bookworm.Services.Data.Contracts.Quotes;
    using Bookworm.Web.ViewModels.DTOs;
    using Microsoft.EntityFrameworkCore;

    using static Bookworm.Common.Constants.ErrorMessagesConstants.QuoteErrorMessagesConstants;
    using static Bookworm.Common.Constants.SuccessMessagesConstants.CrudSuccessMessagesConstants;
    using static Bookworm.Common.Enums.QuoteType;

    public class UploadQuoteService : IUploadQuoteService
    {
        private readonly IDeletableEntityRepository<Quote> quoteRepository;

        public UploadQuoteService(IDeletableEntityRepository<Quote> quoteRepository)
        {
            this.quoteRepository = quoteRepository;
        }

        public async Task<OperationResult> UploadQuoteAsync(
            QuoteDto quoteDto,
            string userId)
        {
            string content = quoteDto.Content.Trim();

            bool quoteExists = await this.quoteRepository
                .AllAsNoTrackingWithDeleted()
                .AnyAsync(x => EF.Functions.Like(x.Content, $"%{content}%"));

            if (quoteExists)
            {
                return OperationResult.Fail(QuoteExistsError);
            }

            var quote = new Quote
            {
                UserId = userId,
                Content = content,
            };

            switch (quoteDto.Type)
            {
                case BookQuote:
                    quote.AuthorName = quoteDto.AuthorName.Trim();
                    quote.BookTitle = quoteDto.BookTitle.Trim();
                    quote.Type = BookQuote;
                    break;
                case MovieQuote:
                    quote.MovieTitle = quoteDto.MovieTitle.Trim();
                    quote.Type = MovieQuote;
                    break;
                case GeneralQuote:
                    quote.AuthorName = quoteDto.AuthorName.Trim();
                    quote.Type = GeneralQuote;
                    break;
                default: return OperationResult.Fail(QuoteInvalidTypeError);
            }

            await this.quoteRepository.AddAsync(quote);
            await this.quoteRepository.SaveChangesAsync();

            return OperationResult.Ok(UploadSuccess);
        }
    }
}
