namespace Bookworm.Services.Data.Tests.Shared
{
    using System;
    using System.Collections.Generic;

    using Bookworm.Common.Enums;
    using Bookworm.Data;
    using Bookworm.Data.Common.Repositories;
    using Bookworm.Data.Models;
    using Microsoft.EntityFrameworkCore;
    using Moq;

    public class DatabaseFixture : IDisposable
    {
        public DatabaseFixture()
        {
            var dbContextOptionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "BookwormDb")
                .Options;

            this.DbContext = new ApplicationDbContext(dbContextOptionsBuilder);

            this.DbContext.Quotes.AddRange(GetQuotes());

            this.DbContext.SaveChanges();
        }

        public ApplicationDbContext DbContext { get; private set; }

        public Mock<IDeletableEntityRepository<Quote>> QuoteRepositoryMock
        {
            get
            {
                var quoteRepoMock = new Mock<IDeletableEntityRepository<Quote>>();

                quoteRepoMock.Setup(x => x.AllAsNoTracking())
                    .Returns(this.DbContext.Quotes.AsQueryable());

                quoteRepoMock
                    .Setup(x => x.AddAsync(It.IsAny<Quote>()))
                    .Callback(async (Quote quote) => await this.DbContext.AddAsync(quote));

                quoteRepoMock
                    .Setup(x => x.SaveChangesAsync())
                    .Callback(async () => await this.DbContext.SaveChangesAsync());

                return quoteRepoMock;
            }
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.DbContext.Dispose();
            }
        }

        private static IEnumerable<Quote> GetQuotes()
        {
            IEnumerable<Quote> quotes =
            [
                new ()
                {
                    Id = 1,
                    Content = "Knowledge is power",
                    AuthorName = "Sir Francis Bacon",
                    Type = QuoteType.GeneralQuote,
                    UserId = "0fc3ea28-3165-440e-947e-670c90562320",
                },
            ];

            return quotes;
        }
    }
}
