namespace Bookworm.Services.Data.Models.Quotes
{
    using System.Threading.Tasks;

    using Bookworm.Data.Common.Repositories;
    using Bookworm.Data.Models;
    using Bookworm.Services.Data.Contracts.Quotes;

    public class UploadQuoteService : IUploadQuoteService
    {
        private readonly IDeletableEntityRepository<Quote> quoteRepository;

        public UploadQuoteService(IDeletableEntityRepository<Quote> quoteRepository)
        {
            this.quoteRepository = quoteRepository;
        }

        public async Task UploadBookQuoteAsync(
            string content,
            string bookTitle,
            string author,
            string userId)
        {
            Quote quote = new Quote()
            {
                Content = content,
                BookTitle = bookTitle,
                AuthorName = author,
                UserId = userId,
            };

            await this.quoteRepository.AddAsync(quote);
            await this.quoteRepository.SaveChangesAsync();
        }

        public async Task UploadGeneralQuoteAsync(
            string content,
            string authorName,
            string userId)
        {
            Quote quote = new Quote()
            {
                Content = content,
                AuthorName = authorName,
                UserId = userId,
            };

            await this.quoteRepository.AddAsync(quote);
            await this.quoteRepository.SaveChangesAsync();
        }

        public async Task UploadMovieQuoteAsync(
            string content,
            string movieTitle,
            string userId)
        {
            Quote quote = new Quote()
            {
                Content = content,
                MovieTitle = movieTitle,
                UserId = userId,
            };

            await this.quoteRepository.AddAsync(quote);
            await this.quoteRepository.SaveChangesAsync();
        }
    }
}
