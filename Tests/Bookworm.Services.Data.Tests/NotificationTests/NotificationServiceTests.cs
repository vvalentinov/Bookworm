namespace Bookworm.Services.Data.Tests.NotificationTests
{
    using System;
    using System.Threading.Tasks;

    using Bookworm.Data;
    using Bookworm.Data.Models;
    using Bookworm.Data.Repositories;
    using Bookworm.Services.Data.Models;
    using Bookworm.Services.Data.Tests.Shared;
    using Microsoft.EntityFrameworkCore;
    using Xunit;

    using static Bookworm.Common.Constants.ErrorMessagesConstants.NotificationsMessagesConstants;

    public class NotificationServiceTests : IClassFixture<DbContextFixture>
    {
        private readonly ApplicationDbContext dbContext;

        public NotificationServiceTests(DbContextFixture dbContextFixture)
        {
            this.dbContext = dbContextFixture.DbContext;
        }

        [Fact]
        public async Task AddNotificationShouldWorkCorrectly()
        {
            var notificationContent = "Some notification content here!";
            var userId = "0fc3ea28-3165-440e-947e-670c90562320";

            await this.GetNotificationService().AddNotificationAsync(notificationContent, userId);

            var newlyAddedNotification = this.GetNotificationRepo()
                .AllAsNoTracking()
                .FirstOrDefaultAsync(n => n.Content == notificationContent && n.UserId == userId);

            Assert.NotNull(newlyAddedNotification);
        }

        [Fact]
        public async Task DeleteUserNotificationShouldWorkCorrectly()
        {
            var notificationId = 2;
            var userId = "f19d077c-ceb8-4fe2-b369-45abd5ffa8f7";

            var count = await this.GetNotificationService().DeleteUserNotificationAsync(userId, notificationId);

            var notification = await this.GetNotificationRepo()
                .AllAsNoTrackingWithDeleted()
                .FirstOrDefaultAsync(n => n.Id == notificationId);

            Assert.NotNull(notification);
            Assert.True(notification.IsDeleted);
            Assert.Equal(1, count);
        }

        [Theory]
        [InlineData(-14)]
        [InlineData(0)]
        [InlineData(10)]
        public async Task DeleteUserNotificationShouldThrowExceptionIfIdIsInvalid(int id)
        {
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(async ()
                => await this.GetNotificationService().DeleteUserNotificationAsync(string.Empty, id));

            Assert.Equal(WrongNotificationIdError, exception.Message);
        }

        [Fact]
        public async Task DeleteUserNotificationShouldThrowExceptionIfUserIsNotOwnerOfNotification()
        {
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(async ()
                => await this.GetNotificationService().DeleteUserNotificationAsync("0fc3ea28-3165-440e-947e-670c90562320", 3));

            Assert.Equal(DeleteNotificationError, exception.Message);
        }

        [Fact]
        public async Task GetUserNotificationsShouldWorkCorrectly()
        {
            var userId = "0fc3ea28-3165-440e-947e-670c90562320";
            var model = await this.GetNotificationService().GetUserNotificationsAsync(userId);

            Assert.Equal(2, model.Notifications.Count);
            Assert.Equal(4, model.Notifications[0].Id);
        }

        [Fact]
        public async Task GetUserNotificationsCountShouldWorkCorrectly()
        {
            var userId = "f19d077c-ceb8-4fe2-b369-45abd5ffa8f7";
            var count = await this.GetNotificationService().GetUserNotificationsCountAsync(userId);

            Assert.Equal(1, count);
        }

        [Fact]
        public async Task MarkOldNotificationsAsDeletedShouldWorkCorrectly()
        {
            await this.GetNotificationService().MarkOldNotificationsAsDeletedAsync();

            var notification = await this.GetNotificationRepo().AllAsNoTrackingWithDeleted()
                .FirstAsync(n => n.Id == 5);

            Assert.True(notification.IsDeleted);
        }

        [Fact]
        public async Task MarkUnreadUserNotificationsAsReadShouldWorkCorrectly()
        {
            var notificationRepo = this.GetNotificationRepo();

            await this.GetNotificationService().MarkUnreadUserNotificationsAsReadAsync("a84ea5dc-a89e-442f-8e53-c874675bb114");

            var notificationOne = await notificationRepo.AllAsNoTracking().FirstOrDefaultAsync(n => n.Id == 8);
            var notificationTwo = await notificationRepo.AllAsNoTracking().FirstOrDefaultAsync(n => n.Id == 9);

            Assert.True(notificationOne.IsRead);
            Assert.True(notificationTwo.IsRead);
        }

        private EfDeletableEntityRepository<Notification> GetNotificationRepo()
            => new EfDeletableEntityRepository<Notification>(this.dbContext);

        private NotificationService GetNotificationService()
            => new NotificationService(this.GetNotificationRepo());
    }
}
