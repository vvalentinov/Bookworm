namespace Bookworm.Services.Data.Models
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    using Bookworm.Data.Common.Repositories;
    using Bookworm.Data.Models;
    using Bookworm.Services.Data.Contracts;
    using Bookworm.Services.Mapping;

    public class QuotesService : IQuotesService
    {
        private readonly IRepository<Quote> quoteRepository;

        public QuotesService(IRepository<Quote> quoteRepository)
        {
            this.quoteRepository = quoteRepository;
        }

        public async Task AddQuoteAsync(
            string content,
            string authorName,
            string bookTitle,
            string movieTitle,
            string userId)
        {
            Quote quote = new Quote()
            {
                Content = content,
                AuthorName = authorName,
                BookName = bookTitle,
                MovieName = movieTitle,
                UserId = userId,
            };

            await this.quoteRepository.AddAsync(quote);
            await this.quoteRepository.SaveChangesAsync();
        }

        public T GetRandomQuote<T>()
        {
            return this.quoteRepository
                        .AllAsNoTracking()
                        .OrderBy(x => Guid.NewGuid())
                        .To<T>()
                        .FirstOrDefault();
        }
    }
}
