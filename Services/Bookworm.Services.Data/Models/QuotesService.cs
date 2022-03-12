namespace Bookworm.Services.Data.Models
{
    using System;
    using System.Linq;

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
