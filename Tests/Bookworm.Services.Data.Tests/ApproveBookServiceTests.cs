﻿//namespace Bookworm.Services.Data.Tests
//{
//    using System.Collections.Generic;
//    using System.Linq;
//    using System.Threading.Tasks;

//    using Bookworm.Data.Common.Repositories;
//    using Bookworm.Data.Models;
//    using Bookworm.Services.Data.Models.Books;
//    using Bookworm.Services.Messaging;
//    using Microsoft.AspNetCore.Identity;
//    using Moq;
//    using Xunit;

//    public class ApproveBookServiceTests
//    {
//        private readonly IList<Book> books;
//        private readonly IList<ApplicationUser> users;

//        // private readonly ApproveBookService approveBookService;
//        public ApproveBookServiceTests()
//        {
//            this.users = new List<ApplicationUser>()
//            {
//                new ApplicationUser()
//                {
//                    Id = "5865cc89-166a-4abe-8749-8498291e9499",
//                },
//            };

//            this.books = new List<Book>()
//            {
//                new Book()
//                {
//                    Id = "77e6fd96-e081-441b-a349-1e6f00e8a5ca",
//                    Title = "Book title",
//                    Description = "Book description",
//                    ImageUrl = "http://example.com/air",
//                    FileUrl = "https://brother.example.org/",
//                    UserId = "5865cc89-166a-4abe-8749-8498291e9499",
//                    IsApproved = false,
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

//            Mock<UserManager<ApplicationUser>> mockUserManager = new Mock<UserManager<ApplicationUser>>(new Mock<IUserStore<ApplicationUser>>().Object, null, null, null, null, null, null, null, null);
//            Mock<IEmailSender> mockEmailSender = new Mock<IEmailSender>();

//            // this.approveBookService = new ApproveBookService(
//            //    mockBookRepo.Object,
//            //    mockUserRepo.Object,
//            //    mockUserManager.Object);
//        }

//        [Fact]
//        public void ApproveBookShouldWorkCorrectly()
//        {
//            // await this.approveBookService.ApproveBook("77e6fd96-e081-441b-a349-1e6f00e8a5ca", "5865cc89-166a-4abe-8749-8498291e9499");
//            Book book = this.books.SingleOrDefault();
//            ApplicationUser user = this.users.SingleOrDefault();
//            Assert.True(book.IsApproved);
//        }
//    }
//}
