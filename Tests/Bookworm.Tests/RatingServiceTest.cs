namespace Bookworm.Tests
{
    using Bookworm.Data.Common.Repositories;
    using Bookworm.Data.Models;
    using Bookworm.Data.Repositories;
    using Bookworm.Services.Data.Contracts;
    using Bookworm.Services.Data.Models;
    using Microsoft.Extensions.DependencyInjection;
    using NUnit.Framework;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class RatingServiceTest
    {
        private ServiceProvider serviceProvider;
        private InMemoryDbContext dbContext;

        [SetUp]
        public async Task Setup()
        {
            dbContext = new InMemoryDbContext();
            var serviceCollection = new ServiceCollection();

            serviceProvider = serviceCollection
                .AddSingleton(sp => dbContext.CreateContext())
                .AddSingleton<IRepository<Rating>, EfRepository<Rating>>()
                .AddSingleton<IDeletableEntityRepository<ApplicationUser>, EfDeletableEntityRepository<ApplicationUser>>()
                .AddSingleton<IRatingsService, RatingsService>()
                .BuildServiceProvider();

            var repo = serviceProvider.GetService<IRepository<Rating>>();
            await SeedDbAsync(repo);
        }

        [Test]
        public void AverageRatingTest()
        {
            var service = serviceProvider.GetService<IRatingsService>();
            var result = service.GetAverageVotes("1");
            Assert.AreEqual(2.66, result);
        }

        [TearDown]
        public void TearDown()
        {
            dbContext.Dispose();
        }

        private async Task SeedDbAsync(IRepository<Rating> repo)
        {
            var rating1 = new Rating()
            {
                BookId = "1",
                UserId = "Ivo",
                Value = 2,
            };

            var rating2 = new Rating()
            {
                BookId = "1",
                UserId = "Pesho",
                Value = 5,
            };

            var rating3 = new Rating()
            {
                BookId = "1",
                UserId = "Ivo",
                Value = 1,
            };

            await repo.AddAsync(rating1);
            await repo.AddAsync(rating2);
            await repo.AddAsync(rating3);
            await repo.SaveChangesAsync();
        }
    }
}
