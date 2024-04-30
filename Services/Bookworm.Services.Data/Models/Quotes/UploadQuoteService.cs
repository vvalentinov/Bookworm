namespace Bookworm.Services.Data.Models.Quotes
{
    using System;
    using System.Threading.Tasks;

    using Bookworm.Common.Enums;
    using Bookworm.Data.Common.Repositories;
    using Bookworm.Data.Models;
    using Bookworm.Services.Data.Contracts.Quotes;
    using Bookworm.Web.ViewModels.DTOs;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.EntityFrameworkCore;

    using static Bookworm.Common.Constants.ErrorMessagesConstants.IdentityErrorMessagesConstants;
    using static Bookworm.Common.Constants.ErrorMessagesConstants.QuoteErrorMessagesConstants;

    public class UploadQuoteService : IUploadQuoteService
    {
        private readonly IDeletableEntityRepository<Quote> quoteRepository;
        private readonly UserManager<ApplicationUser> userManager;

        public UploadQuoteService(
            IDeletableEntityRepository<Quote> quoteRepository,
            UserManager<ApplicationUser> userManager)
        {
            this.quoteRepository = quoteRepository;
            this.userManager = userManager;
        }

        public async Task UploadQuoteAsync(QuoteDto quoteDto, string userId)
        {
            var user = await this.userManager.FindByIdAsync(userId) ??
                throw new InvalidOperationException(UserWrongIdError);

            string content = quoteDto.Content.Trim();

            bool quoteExist = await this.quoteRepository
                .AllAsNoTracking()
                .AnyAsync(x => x.Content.ToLower() == content.ToLower());

            if (quoteExist)
            {
                throw new InvalidOperationException(QuoteExistsError);
            }

            var quote = new Quote { Content = content, UserId = user.Id };

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
