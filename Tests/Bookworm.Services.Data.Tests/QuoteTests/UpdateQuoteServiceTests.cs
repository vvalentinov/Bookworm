namespace Bookworm.Services.Data.Tests.QuoteTests
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    using Bookworm.Common.Enums;
    using Bookworm.Data;
    using Bookworm.Data.Common.Repositories;
    using Bookworm.Data.Models;
    using Bookworm.Data.Repositories;
    using Bookworm.Services.Data.Models;
    using Bookworm.Services.Data.Models.Quotes;
    using Bookworm.Services.Data.Tests.Shared;
    using Bookworm.Web.ViewModels.DTOs;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.EntityFrameworkCore;
    using Moq;
    using Xunit;

    using static Bookworm.Common.Constants.DataConstants.QuoteDataConstants;
    using static Bookworm.Common.Constants.ErrorMessagesConstants.QuoteErrorMessagesConstants;

    [Collection("Database")]
    public class UpdateQuoteServiceTests
    {
        private readonly UpdateQuoteService updateQuoteService;
        private readonly IDeletableEntityRepository<Quote> quoteRepo;
        private readonly Mock<UserManager<ApplicationUser>> userManagerMock;
        private readonly ApplicationDbContext dbContext;

        public UpdateQuoteServiceTests(DbContextFixture dbContextFixture)
        {
            this.dbContext = dbContextFixture.DbContext;
            this.quoteRepo = new EfDeletableEntityRepository<Quote>(dbContextFixture.DbContext);

            var store = new Mock<IUserStore<ApplicationUser>>();
            this.userManagerMock = new Mock<UserManager<ApplicationUser>>(
                store.Object, null, null, null, null, null, null, null, null);
            this.userManagerMock.Setup(x => x.FindByIdAsync(It.IsAny<string>()))
                .ReturnsAsync((string userId) => this.dbContext.Users.FirstOrDefault(u => u.Id == userId));

            var usersServiceMock = new Mock<UsersService>(this.userManagerMock.Object).Object;

            this.updateQuoteService = new UpdateQuoteService(this.quoteRepo, usersServiceMock);
        }

        [Fact]
        public async Task ApproveQuoteShouldWorkCorrectly()
        {
            var user = await this.userManagerMock.Object
                .FindByIdAsync("0fc3ea28-3165-440e-947e-670c90562320");

            var expectedUserPoints = user.Points + QuoteUploadPoints;

            await this.updateQuoteService.ApproveQuoteAsync(1);

            var quote = await this.quoteRepo.AllAsNoTracking().FirstAsync(q => q.Id == 1);

            Assert.True(quote.IsApproved);
            Assert.Equal(expectedUserPoints, user.Points);
        }

        [Fact]
        public async Task ApproveQuoteShouldNotIncreaseUserPointsIfQuoteIsNotApproved()
        {
            var user = await this.userManagerMock.Object
                .FindByIdAsync("a84ea5dc-a89e-442f-8e53-c874675bb114");

            var expectedUserPoints = user.Points;

            await this.updateQuoteService.ApproveQuoteAsync(5);

            var quote = await this.quoteRepo.AllAsNoTracking().FirstAsync(q => q.Id == 5);

            Assert.True(quote.IsApproved);
            Assert.Equal(expectedUserPoints, user.Points);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-10)]
        [InlineData(11)]
        public async Task ApproveQuoteShouldThrowExceptionIfIdIsInvalid(int id)
        {
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(async ()
                => await this.updateQuoteService.ApproveQuoteAsync(id));

            Assert.Equal(QuoteWrongIdError, exception.Message);
        }

        [Fact]
        public async Task DeleteQuoteShouldWorkCorrectlyIfUserIsCreator()
        {
            var user = await this.userManagerMock.Object
               .FindByIdAsync("0fc3ea28-3165-440e-947e-670c90562320");
            var expectedPoints = user.Points - QuoteUploadPoints;

            await this.updateQuoteService.DeleteQuoteAsync(2, "0fc3ea28-3165-440e-947e-670c90562320");

            var quote = await this.quoteRepo.AllAsNoTrackingWithDeleted().FirstAsync(q => q.Id == 2);
            Assert.True(quote.IsDeleted);
            Assert.Equal(expectedPoints, user.Points);
        }

        [Fact]
        public async Task DeleteQuoteShouldWorkCorrectlyIfUserIsNotCreatorButAdministrator()
        {
            var user = await this.userManagerMock.Object
               .FindByIdAsync("f19d077c-ceb8-4fe2-b369-45abd5ffa8f7");
            var expectedPoints = user.Points - QuoteUploadPoints;

            await this.updateQuoteService.DeleteQuoteAsync(3, "0fc3ea28-3165-440e-947e-670c90562320", true);

            var quote = await this.quoteRepo.AllAsNoTrackingWithDeleted().FirstAsync(q => q.Id == 3);
            Assert.True(quote.IsDeleted);
            Assert.Equal(expectedPoints, user.Points);
        }

        [Fact]
        public async Task DeleteQuoteShouldNotReduceUserPointsIfQuoteIsNotApproved()
        {
            var user = await this.userManagerMock.Object
               .FindByIdAsync("a84ea5dc-a89e-442f-8e53-c874675bb114");
            var expectedPoints = user.Points;

            await this.updateQuoteService.DeleteQuoteAsync(4, "a84ea5dc-a89e-442f-8e53-c874675bb114");

            var quote = await this.quoteRepo.AllAsNoTrackingWithDeleted().FirstAsync(q => q.Id == 4);
            Assert.True(quote.IsDeleted);
            Assert.Equal(expectedPoints, user.Points);
        }

        [Fact]
        public async Task DeleteQuoteShouldThrowExceptionIfIdIsInvalid()
        {
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(async ()
                => await this.updateQuoteService
                .DeleteQuoteAsync(0, "0fc3ea28-3165-440e-947e-670c90562320"));

            Assert.Equal(QuoteWrongIdError, exception.Message);
        }

        [Fact]
        public async Task DeleteQuoteShouldThrowExceptionIfUserIsNotCreatorOrAdmin()
        {
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(async ()
                => await this.updateQuoteService
                .DeleteQuoteAsync(2, "f19d077c-ceb8-4fe2-b369-45abd5ffa8f7"));
        }

        [Fact]
        public async Task UndeleteQuoteShouldWorkCorrectly()
        {
            await this.updateQuoteService.UndeleteQuoteAsync(6);

            var quote = await this.quoteRepo
                .AllAsNoTrackingWithDeleted().FirstAsync(q => q.Id == 6);

            Assert.False(quote.IsDeleted);
        }

        [Fact]
        public async Task UndeleteQuoteShouldThrowExceptionIfIdIsInvalid()
        {
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(async ()
                => await this.updateQuoteService.UndeleteQuoteAsync(11));

            Assert.Equal(QuoteWrongIdError, exception.Message);
        }

        [Fact]
        public async Task UnapproveQuoteShouldWorkCorrectly()
        {
            var user = await this.userManagerMock.Object
                .FindByIdAsync("a84ea5dc-a89e-442f-8e53-c874675bb114");

            var expectedUserPoints = user.Points - QuoteUploadPoints;

            await this.updateQuoteService.UnapproveQuoteAsync(7);

            var quote = await this.quoteRepo.AllAsNoTracking().FirstAsync(q => q.Id == 7);

            Assert.False(quote.IsApproved);
            Assert.Equal(expectedUserPoints, user.Points);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(11)]
        [InlineData(-17)]
        public async Task UnapproveQuoteShouldThrowExceptionIfIdIsInvalid(int id)
        {
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(async ()
                => await this.updateQuoteService.UnapproveQuoteAsync(id));

            Assert.Equal(QuoteWrongIdError, exception.Message);
        }

        [Fact]
        public async Task EditQuoteShouldWorkCorrectlyWhenQuoteIsApproved()
        {
            var user = await this.userManagerMock.Object
                .FindByIdAsync("0fc3ea28-3165-440e-947e-670c90562320");

            var expectedUserPoints = user.Points - QuoteUploadPoints;

            var quoteDto = new QuoteDto
            {
                Id = 8,
                Content = "Updated!",
                MovieTitle = "Updated!",
                Type = QuoteType.MovieQuote,
            };

            await this.updateQuoteService.EditQuoteAsync(
                quoteDto, "0fc3ea28-3165-440e-947e-670c90562320");

            var quote = await this.quoteRepo
                .AllAsNoTracking().FirstAsync(q => q.Id == quoteDto.Id);

            Assert.False(quote.IsApproved);
            Assert.Equal(expectedUserPoints, user.Points);
            Assert.Equal(quoteDto.Content, quote.Content);
            Assert.Equal(quoteDto.MovieTitle, quote.MovieTitle);
        }

        [Fact]
        public async Task EditQuoteShouldWorkCorrectlyWhenQuoteIsNotApproved()
        {
            var user = await this.userManagerMock.Object
                .FindByIdAsync("f19d077c-ceb8-4fe2-b369-45abd5ffa8f7");

            var expectedUserPoints = user.Points;

            var quoteDto = new QuoteDto
            {
                Id = 9,
                Content = "Updated! Updated!",
                MovieTitle = "Updated! Updated!",
                Type = QuoteType.MovieQuote,
            };

            await this.updateQuoteService.EditQuoteAsync(
                quoteDto, "f19d077c-ceb8-4fe2-b369-45abd5ffa8f7");

            var quote = await this.quoteRepo
                .AllAsNoTracking().FirstAsync(q => q.Id == quoteDto.Id);

            Assert.False(quote.IsApproved);
            Assert.Equal(expectedUserPoints, user.Points);
            Assert.Equal(quoteDto.Content, quote.Content);
            Assert.Equal(quoteDto.MovieTitle, quote.MovieTitle);
        }

        [Theory]
        [InlineData(11)]
        [InlineData(-15)]
        [InlineData(100)]
        public async Task EditQuoteShouldThrowExceptionIfIdIsInvalid(int id)
        {
            var quoteDto = new QuoteDto
            {
                Id = id,
                Content = "Some updated content!",
                MovieTitle = "Some updated movie title!",
                Type = QuoteType.MovieQuote,
            };

            var exception = await Assert.ThrowsAsync<InvalidOperationException>(async ()
                => await this.updateQuoteService.EditQuoteAsync(
                    quoteDto, "d6fa29a2-a8f4-4f77-ada2-8e435649c483"));

            Assert.Equal(QuoteWrongIdError, exception.Message);
        }

        [Fact]
        public async Task EditQuoteShouldThrowExceptionIfUserIsNotCreatorOfQuote()
        {
            var quoteDto = new QuoteDto { Id = 9 };

            var exception = await Assert.ThrowsAsync<InvalidOperationException>(async ()
                => await this.updateQuoteService.EditQuoteAsync(
                    quoteDto, "d6fa29a2-a8f4-4f77-ada2-8e435649c483"));

            Assert.Equal(QuoteEditError, exception.Message);
        }

        [Fact]
        public async Task EditQuoteShouldThrowExceptionIfQuoteTypeIsInvalid()
        {
            var quoteDto = new QuoteDto { Id = 9, Type = QuoteType.GeneralQuote };

            var exception = await Assert.ThrowsAsync<InvalidOperationException>(async ()
                => await this.updateQuoteService.EditQuoteAsync(
                    quoteDto, "f19d077c-ceb8-4fe2-b369-45abd5ffa8f7"));

            Assert.Equal(QuoteInvalidTypeError, exception.Message);
        }
    }
}
