﻿//namespace Bookworm.Services.Data.Tests
//{
//    using System.Collections.Generic;
//    using System.Linq;
//    using System.Threading.Tasks;

//    using Bookworm.Data.Common.Repositories;
//    using Bookworm.Data.Models;
//    using Bookworm.Services.Data.Contracts;
//    using Bookworm.Services.Data.Models;
//    using Bookworm.Web.ViewModels.Users;
//    using Microsoft.AspNetCore.Identity;
//    using Microsoft.Extensions.Configuration;
//    using Moq;
//    using Xunit;

//    public class UsersServiceTests
//    {
//        private readonly IList<ApplicationUser> users;
//        private readonly IList<Book> books;
//        private readonly IList<Quote> quotes;

//        private readonly UsersService usersService;

//        public UsersServiceTests()
//        {
//            this.users = new List<ApplicationUser>()
//            {
//                new ApplicationUser()
//                {
//                    Id = "cc741abb-7aba-42eb-bc02-d64d931af949",
//                    UserName = "First Username",
//                },
//                new ApplicationUser()
//                {
//                    Id = "cf24e05f-3520-4f65-ac00-45bc33adb0b2",
//                    UserName = "Second Username",
//                },
//                new ApplicationUser()
//                {
//                    Id = "87955cda-29b4-460c-b8f3-d0656388787a",
//                    UserName = "Third Username",
//                },
//            };

//            this.quotes = new List<Quote>()
//            {
//                new Quote()
//                {
//                    Content = "First Quote Content",
//                    UserId = "cc741abb-7aba-42eb-bc02-d64d931af949",
//                },
//                new Quote()
//                {
//                    Content = "Second Quote Content",
//                    UserId = "87955cda-29b4-460c-b8f3-d0656388787a",
//                },
//            };

//            this.books = new List<Book>()
//            {
//                new Book()
//                {
//                    Id = "77e6fd96-e081-441b-a349-1e6f00e8a5ca",
//                    Title = "First book title",
//                    Description = "First book description",
//                    ImageUrl = "http://example.com/air",
//                    FileUrl = "https://brother.example.org/",
//                    UserId = "cc741abb-7aba-42eb-bc02-d64d931af949",
//                },
//                new Book()
//                {
//                    Id = "8e5fca84-9b02-4f98-9ca1-9268f2bfb62d",
//                    Title = "Second book title",
//                    Description = "Second book description",
//                    ImageUrl = "http://baseball.example.com/",
//                    FileUrl = "https://act.example.com/",
//                    UserId = "87955cda-29b4-460c-b8f3-d0656388787a",
//                },
//            };

//            Mock<IDeletableEntityRepository<ApplicationUser>> mockUserRepo = new Mock<IDeletableEntityRepository<ApplicationUser>>();
//            mockUserRepo.Setup(x => x.AllAsNoTracking()).Returns(this.users.AsQueryable());
//            mockUserRepo.Setup(x => x.All()).Returns(this.users.AsQueryable());
//            mockUserRepo.Setup(x => x.AddAsync(It.IsAny<ApplicationUser>()))
//                .Callback((ApplicationUser user) => this.users.Add(user));

//            Mock<IDeletableEntityRepository<Book>> mockBookRepo = new Mock<IDeletableEntityRepository<Book>>();
//            mockBookRepo.Setup(x => x.AllAsNoTracking()).Returns(this.books.AsQueryable());
//            mockBookRepo.Setup(x => x.All()).Returns(this.books.AsQueryable());
//            mockBookRepo.Setup(x => x.AddAsync(It.IsAny<Book>()))
//                .Callback((Book book) => this.books.Add(book));

//            Mock<IRepository<Quote>> mockQuoteRepo = new Mock<IRepository<Quote>>();
//            mockQuoteRepo.Setup(x => x.AllAsNoTracking()).Returns(this.quotes.AsQueryable());
//            mockQuoteRepo.Setup(x => x.All()).Returns(this.quotes.AsQueryable());
//            mockQuoteRepo.Setup(x => x.AddAsync(It.IsAny<Quote>()))
//                .Callback((Quote quote) => this.quotes.Add(quote));

//            Mock<IBlobService> mockBlobService = new Mock<IBlobService>();
//            Mock<UserManager<ApplicationUser>> mockUserManager = new Mock<UserManager<ApplicationUser>>(new Mock<IUserStore<ApplicationUser>>().Object, null, null, null, null, null, null, null, null);
//            Mock<IConfiguration> mockConfiguration = new Mock<IConfiguration>();

//            this.usersService = new UsersService(
//                mockUserRepo.Object,
//                mockBookRepo.Object,
//                mockQuoteRepo.Object,
//                mockUserManager.Object,
//                mockConfiguration.Object,
//                mockBlobService.Object);
//        }

//        [Fact]
//        public void GetUserWithIdShouldReturnCorrectUser()
//        {
//            ApplicationUser user = this.usersService.GetUserWithId("cc741abb-7aba-42eb-bc02-d64d931af949");

//            Assert.NotNull(user);
//            Assert.Equal("cc741abb-7aba-42eb-bc02-d64d931af949", user.Id);
//        }

//        [Fact]
//        public async Task GetUserModelShouldWorkCorrectly()
//        {
//            UserViewModel user = await this.usersService.GetUserModelWithId("cc741abb-7aba-42eb-bc02-d64d931af949");

//            Assert.NotNull(user);
//            Assert.IsType<UserViewModel>(user);
//            Assert.Equal("cc741abb-7aba-42eb-bc02-d64d931af949", user.Id);
//        }

//        [Fact]
//        public void GetUsersStatisticsShouldWorkCorrectly()
//        {
//            List<UserStatisticsViewModel> userStatistics = this.usersService.GetUsersStatistics().ToList();

//            Assert.Equal(3, userStatistics.Count);

//            Assert.Equal("87955cda-29b4-460c-b8f3-d0656388787a", userStatistics[0].Id);
//            Assert.Equal("Third Username", userStatistics[0].UserName);
//            Assert.Equal(25, userStatistics[0].Points);

//            Assert.Equal("cf24e05f-3520-4f65-ac00-45bc33adb0b2", userStatistics[1].Id);
//            Assert.Equal("Second Username", userStatistics[1].UserName);
//            Assert.Equal(12, userStatistics[1].Points);

//            Assert.Equal("cc741abb-7aba-42eb-bc02-d64d931af949", userStatistics[2].Id);
//            Assert.Equal("First Username", userStatistics[2].UserName);
//            Assert.Equal(5, userStatistics[2].Points);
//        }

//        [Fact]
//        public void GetUsersShouldWorkCorrectly()
//        {
//            IEnumerable<UsersListViewModel> users = this.usersService.GetUsers();

//            Assert.Equal(3, users.Count());

//            foreach (var user in users)
//            {
//                Assert.IsType<UsersListViewModel>(user);
//            }
//        }
//    }
//}
