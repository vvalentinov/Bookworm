namespace Bookworm.Services.Data.Models.Quotes
{
    using System.Threading.Tasks;

    using Bookworm.Data.Common.Repositories;
    using Bookworm.Data.Models;
    using Bookworm.Services.Data.Contracts.Quotes;
    using Microsoft.EntityFrameworkCore;

    public class CheckIfQuoteExistsService : ICheckIfQuoteExistsService
    {
        private readonly IDeletableEntityRepository<Quote> quoteRepository;

        public CheckIfQuoteExistsService(IDeletableEntityRepository<Quote> quoteRepository)
        {
            this.quoteRepository = quoteRepository;
        }

        public async Task<bool> QuoteExistsAsync(string content)
        {
            return await this.quoteRepository
                .AllAsNoTracking()
                .AnyAsync(x => x.Content.ToLower().Contains(content.Trim().ToLower()));
        }
    }
}
