namespace Bookworm.Services.Data.Tests
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    using Bookworm.Data.Common.Repositories;
    using Bookworm.Data.Models;
    using Bookworm.Services.Data.Contracts;
    using Bookworm.Services.Data.Models;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.Extensions.Configuration;
    using Moq;
    using Xunit;

    public class UsersServiceTests
    {
        public UsersServiceTests()
        {
            this.Configuration = this.SetConfiguration();
        }

        protected IConfigurationRoot Configuration { get; set; }

        [Fact]
        public void GetUsersShouldWorkCorrectly()
        {
            List<ApplicationUser> usersList = new List<ApplicationUser>()
            {
                new ApplicationUser()
                {
                    Id = "1",
                },
                new ApplicationUser()
                {
                    Id = "2",
                },
                new ApplicationUser()
                {
                    Id = "3",
                },
            };
            Mock<IDeletableEntityRepository<ApplicationUser>> mockUserRepo = new Mock<IDeletableEntityRepository<ApplicationUser>>();
            mockUserRepo.Setup(x => x.AllAsNoTracking()).Returns(usersList.AsQueryable());
            mockUserRepo.Setup(x => x.AddAsync(It.IsAny<ApplicationUser>()))
                .Callback((ApplicationUser user) => usersList.Add(user));

            Mock<UserManager<ApplicationUser>> userManager = new Mock<UserManager<ApplicationUser>>(
                new Mock<IUserStore<ApplicationUser>>().Object, null, null, null, null, null, null, null, null);
            var blobService = new Mock<IBlobService>();

            UsersService service = new UsersService(mockUserRepo.Object, userManager.Object, this.Configuration, blobService.Object);

            var users = service.GetUsers();

            Assert.Equal(3, usersList.Count);
        }

        private IConfigurationRoot SetConfiguration()
        {
            return new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile(
                 path: "appsettings.json",
                 optional: false,
                 reloadOnChange: true)
           .Build();
        }
    }
}
