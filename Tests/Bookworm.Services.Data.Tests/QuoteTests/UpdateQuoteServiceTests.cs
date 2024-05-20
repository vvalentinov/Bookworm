namespace Bookworm.Services.Data.Tests.QuoteTests
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    using Bookworm.Common.Enums;
    using Bookworm.Data;
    using Bookworm.Data.Models;
    using Bookworm.Data.Repositories;
    using Bookworm.Services.Data.Contracts;
    using Bookworm.Services.Data.Models;
    using Bookworm.Services.Data.Models.Quotes;
    using Bookworm.Services.Data.Tests.Shared;
    using Bookworm.Services.Messaging.Hubs;
    using Bookworm.Web.ViewModels.DTOs;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.SignalR;
    using Microsoft.EntityFrameworkCore;
    using Moq;
    using Xunit;

    using static Bookworm.Common.Constants.DataConstants.QuoteDataConstants;
    using static Bookworm.Common.Constants.ErrorMessagesConstants.QuoteErrorMessagesConstants;

    public class UpdateQuoteServiceTests : IClassFixture<DbContextFixture>
    {
        private readonly ApplicationDbContext dbContext;

        public UpdateQuoteServiceTests(DbContextFixture dbContextFixture)
        {
            this.dbContext = dbContextFixture.DbContext;
        }

        [Fact]
        public async Task ApproveQuoteShouldWorkCorrectly()
        {
            var quoteRepo = this.GetQuoteRepo();
            var userManager = this.GetUserManager();
            var updateQuoteService = this.GetUpdateQuoteService();

            var user = await userManager.FindByIdAsync("0fc3ea28-3165-440e-947e-670c90562320");

            var expectedUserPoints = user.Points + QuoteUploadPoints;

            await updateQuoteService.ApproveQuoteAsync(1);

            var quote = await quoteRepo.AllAsNoTracking().FirstAsync(q => q.Id == 1);

            Assert.True(quote.IsApproved);
            Assert.Equal(expectedUserPoints, user.Points);
        }

        [Fact]
        public async Task ApproveQuoteShouldNotIncreaseUserPointsIfQuoteIsApproved()
        {
            var quoteRepo = this.GetQuoteRepo();
            var userManager = this.GetUserManager();
            var updateQuoteService = this.GetUpdateQuoteService();

            var user = await userManager.FindByIdAsync("b1a9a91f-f7b1-4459-9864-4a4fdd6077c5");

            var expectedUserPoints = user.Points;

            await updateQuoteService.ApproveQuoteAsync(14);

            var quote = await quoteRepo.AllAsNoTracking().FirstAsync(q => q.Id == 14);

            Assert.True(quote.IsApproved);
            Assert.Equal(expectedUserPoints, user.Points);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-10)]
        [InlineData(20)]
        public async Task ApproveQuoteShouldThrowExceptionIfIdIsInvalid(int id)
        {
            var updateQuoteService = this.GetUpdateQuoteService();

            var exception = await Assert.ThrowsAsync<InvalidOperationException>(async ()
                => await updateQuoteService.ApproveQuoteAsync(id));

            Assert.Equal(QuoteWrongIdError, exception.Message);
        }

        [Fact]
        public async Task DeleteQuoteShouldWorkCorrectlyIfUserIsCreator()
        {
            var userId = "a84ea5dc-a89e-442f-8e53-c874675bb114";
            var user = await this.GetUserManager().FindByIdAsync(userId);
            var expectedPoints = user.Points - QuoteUploadPoints;

            await this.GetUpdateQuoteService().DeleteQuoteAsync(5, userId);

            var quote = await this.GetQuoteRepo()
                .AllAsNoTrackingWithDeleted()
                .FirstAsync(q => q.Id == 5);

            Assert.True(quote.IsDeleted);
            Assert.Equal(expectedPoints, user.Points);
        }

        [Fact]
        public async Task DeleteQuoteShouldWorkCorrectlyIfUserIsNotCreatorButAdministrator()
        {
            var quoteRepo = this.GetQuoteRepo();
            var userManager = this.GetUserManager();
            var updateQuoteService = this.GetUpdateQuoteService();

            var user = await userManager.FindByIdAsync("f19d077c-ceb8-4fe2-b369-45abd5ffa8f7");
            var expectedPoints = user.Points - QuoteUploadPoints;

            await updateQuoteService.DeleteQuoteAsync(3, "0fc3ea28-3165-440e-947e-670c90562320", true);

            var quote = await quoteRepo.AllAsNoTrackingWithDeleted().FirstAsync(q => q.Id == 3);
            Assert.True(quote.IsDeleted);
            Assert.Equal(expectedPoints, user.Points);
        }

        [Fact]
        public async Task DeleteQuoteShouldNotReduceUserPointsIfQuoteIsNotApproved()
        {
            var quoteRepo = this.GetQuoteRepo();
            var userManager = this.GetUserManager();
            var updateQuoteService = this.GetUpdateQuoteService();

            var user = await userManager
               .FindByIdAsync("f19d077c-ceb8-4fe2-b369-45abd5ffa8f7");
            var expectedPoints = user.Points;

            await updateQuoteService.DeleteQuoteAsync(9, "f19d077c-ceb8-4fe2-b369-45abd5ffa8f7");

            var quote = await quoteRepo.AllAsNoTrackingWithDeleted().FirstAsync(q => q.Id == 9);
            Assert.True(quote.IsDeleted);
            Assert.Equal(expectedPoints, user.Points);
        }

        [Fact]
        public async Task DeleteQuoteShouldThrowExceptionIfIdIsInvalid()
        {
            var updateQuoteService = this.GetUpdateQuoteService();

            var exception = await Assert.ThrowsAsync<InvalidOperationException>(async ()
                => await updateQuoteService.DeleteQuoteAsync(0, string.Empty));

            Assert.Equal(QuoteWrongIdError, exception.Message);
        }

        [Fact]
        public async Task DeleteQuoteShouldThrowExceptionIfUserIsNotCreatorOrAdmin()
        {
            var updateQuoteService = this.GetUpdateQuoteService();

            var exception = await Assert.ThrowsAsync<InvalidOperationException>(async ()
                => await updateQuoteService.DeleteQuoteAsync(
                    quoteId: 2,
                    userId: "f19d077c-ceb8-4fe2-b369-45abd5ffa8f7",
                    isCurrUserAdmin: false));
        }

        [Fact]
        public async Task UndeleteQuoteShouldWorkCorrectly()
        {
            var quoteRepo = this.GetQuoteRepo();
            var updateQuoteService = this.GetUpdateQuoteService();

            await updateQuoteService.UndeleteQuoteAsync(6);

            var quote = await quoteRepo.AllAsNoTrackingWithDeleted().FirstAsync(q => q.Id == 6);

            Assert.False(quote.IsDeleted);
        }

        [Fact]
        public async Task UndeleteQuoteShouldThrowExceptionIfIdIsInvalid()
        {
            var updateQuoteService = this.GetUpdateQuoteService();

            var exception = await Assert.ThrowsAsync<InvalidOperationException>(async ()
                => await updateQuoteService.UndeleteQuoteAsync(20));

            Assert.Equal(QuoteWrongIdError, exception.Message);
        }

        [Fact]
        public async Task UnapproveQuoteShouldWorkCorrectly()
        {
            var quoteRepo = this.GetQuoteRepo();
            var userManager = this.GetUserManager();
            var updateQuoteService = this.GetUpdateQuoteService();

            var user = await userManager.FindByIdAsync("a84ea5dc-a89e-442f-8e53-c874675bb114");

            var expectedUserPoints = user.Points - QuoteUploadPoints;

            await updateQuoteService.UnapproveQuoteAsync(7);

            var quote = await quoteRepo.AllAsNoTracking().FirstAsync(q => q.Id == 7);

            Assert.False(quote.IsApproved);
            Assert.Equal(expectedUserPoints, user.Points);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(20)]
        [InlineData(-17)]
        public async Task UnapproveQuoteShouldThrowExceptionIfIdIsInvalid(int id)
        {
            var updateQuoteService = this.GetUpdateQuoteService();

            var exception = await Assert.ThrowsAsync<InvalidOperationException>(async ()
                => await updateQuoteService.UnapproveQuoteAsync(id));

            Assert.Equal(QuoteWrongIdError, exception.Message);
        }

        [Theory]
        [InlineData(QuoteType.BookQuote, 11)]
        [InlineData(QuoteType.MovieQuote, 8)]
        [InlineData(QuoteType.GeneralQuote, 2)]
        public async Task EditQuoteShouldWorkCorrectlyWhenQuoteIsApproved(QuoteType type, int quoteId)
        {
            string userId = "0fc3ea28-3165-440e-947e-670c90562320";
            var user = await this.GetUserManager().FindByIdAsync(userId);
            var expectedUserPoints = user.Points - QuoteUploadPoints;
            var quoteDto = new QuoteDto
            {
                Id = quoteId,
                Content = "Updated!",
                Type = type,
            };

            switch (type)
            {
                case QuoteType.BookQuote:
                    quoteDto.AuthorName = "Updated Author Name!";
                    quoteDto.BookTitle = "Updated Book Title!";
                    break;
                case QuoteType.MovieQuote:
                    quoteDto.MovieTitle = "Updated Movie Title!";
                    break;
                case QuoteType.GeneralQuote:
                    quoteDto.AuthorName = "Updated Author Name!";
                    break;
            }

            await this.GetUpdateQuoteService().EditQuoteAsync(quoteDto, userId);

            var quote = await this.GetQuoteRepo().AllAsNoTracking()
                .FirstAsync(q => q.Id == quoteDto.Id);

            Assert.False(quote.IsApproved);
            Assert.Equal(expectedUserPoints, user.Points);
            Assert.Equal(quoteDto.Content, quote.Content);
            Assert.Equal(quoteDto.MovieTitle, quote.MovieTitle);
            Assert.Equal(quoteDto.AuthorName, quote.AuthorName);
            Assert.Equal(quoteDto.BookTitle, quote.BookTitle);
        }

        [Fact]
        public async Task EditQuoteShouldWorkCorrectlyWhenQuoteIsNotApproved()
        {
            var quoteRepo = this.GetQuoteRepo();
            var userManager = this.GetUserManager();
            var updateQuoteService = this.GetUpdateQuoteService();

            var user = await userManager.FindByIdAsync("f19d077c-ceb8-4fe2-b369-45abd5ffa8f7");

            var expectedUserPoints = user.Points;

            var quoteDto = new QuoteDto
            {
                Id = 9,
                Content = "Updated! Updated!",
                MovieTitle = "Updated! Updated!",
                Type = QuoteType.MovieQuote,
            };

            await updateQuoteService.EditQuoteAsync(
                quoteDto, "f19d077c-ceb8-4fe2-b369-45abd5ffa8f7");

            var quote = await quoteRepo
                .AllAsNoTracking().FirstAsync(q => q.Id == quoteDto.Id);

            Assert.False(quote.IsApproved);
            Assert.Equal(expectedUserPoints, user.Points);
            Assert.Equal(quoteDto.Content, quote.Content);
            Assert.Equal(quoteDto.MovieTitle, quote.MovieTitle);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-15)]
        [InlineData(100)]
        public async Task EditQuoteShouldThrowExceptionIfIdIsInvalid(int id)
        {
            var updateQuoteService = this.GetUpdateQuoteService();

            var quoteDto = new QuoteDto
            {
                Id = id,
                Content = "Some updated content!",
                MovieTitle = "Some updated movie title!",
                Type = QuoteType.MovieQuote,
            };

            var exception = await Assert.ThrowsAsync<InvalidOperationException>(async ()
                => await updateQuoteService.EditQuoteAsync(
                    quoteDto, "d6fa29a2-a8f4-4f77-ada2-8e435649c483"));

            Assert.Equal(QuoteWrongIdError, exception.Message);
        }

        [Fact]
        public async Task EditQuoteShouldThrowExceptionIfUserIsNotCreatorOfQuote()
        {
            var updateQuoteService = this.GetUpdateQuoteService();

            var quoteDto = new QuoteDto { Id = 9 };

            var exception = await Assert.ThrowsAsync<InvalidOperationException>(async ()
                => await updateQuoteService.EditQuoteAsync(
                    quoteDto, "d6fa29a2-a8f4-4f77-ada2-8e435649c483"));

            Assert.Equal(QuoteEditError, exception.Message);
        }

        [Fact]
        public async Task EditQuoteShouldThrowExceptionIfQuoteTypeIsInvalid()
        {
            var updateQuoteService = this.GetUpdateQuoteService();

            var quoteDto = new QuoteDto { Id = 9, Type = QuoteType.GeneralQuote };

            var exception = await Assert.ThrowsAsync<InvalidOperationException>(async ()
                => await updateQuoteService.EditQuoteAsync(
                    quoteDto, "f19d077c-ceb8-4fe2-b369-45abd5ffa8f7"));

            Assert.Equal(QuoteInvalidTypeError, exception.Message);
        }

        private EfDeletableEntityRepository<Quote> GetQuoteRepo()
            => new EfDeletableEntityRepository<Quote>(this.dbContext);

        private UserManager<ApplicationUser> GetUserManager()
        {
            var store = new Mock<IUserStore<ApplicationUser>>();
            var userManagerMock = new Mock<UserManager<ApplicationUser>>(
                store.Object, null, null, null, null, null, null, null, null);
            userManagerMock.Setup(x => x.FindByIdAsync(It.IsAny<string>()))
                .ReturnsAsync((string userId) => this.dbContext.Users.FirstOrDefault(u => u.Id == userId));

            return userManagerMock.Object;
        }

        private UpdateQuoteService GetUpdateQuoteService()
            => new UpdateQuoteService(
                this.GetQuoteRepo(),
                new Mock<UsersService>(this.GetUserManager()).Object,
                new Mock<IHubContext<NotificationHub>>().Object,
                new Mock<INotificationService>().Object);
    }
}
