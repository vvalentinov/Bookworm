namespace Bookworm.Services.Data.Models
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    using Bookworm.Common;
    using Bookworm.Data.Common.Repositories;
    using Bookworm.Data.Models;
    using Bookworm.Services.Data.Contracts;
    using Bookworm.Services.Mapping;
    using Bookworm.Web.ViewModels.Notification;
    using Microsoft.EntityFrameworkCore;

    using static Bookworm.Common.Constants.ErrorMessagesConstants.NotificationsMessagesConstants;

    public class NotificationService : INotificationService
    {
        private readonly IDeletableEntityRepository<Notification> notificationRepo;

        public NotificationService(IDeletableEntityRepository<Notification> notificationRepo)
        {
            this.notificationRepo = notificationRepo;
        }

        public async Task<OperationResult> AddNotificationAsync(
            string content,
            string userId)
        {
            var notification = new Notification
            {
                UserId = userId,
                Content = content,
            };

            await this.notificationRepo.AddAsync(notification);
            await this.notificationRepo.SaveChangesAsync();

            return OperationResult.Ok();
        }

        public async Task<OperationResult<int>> DeleteUserNotificationAsync(
            string userId,
            int notificationId)
        {
            var notification = await this.notificationRepo
                .All()
                .FirstOrDefaultAsync(n => n.Id == notificationId);

            if (notification == null)
            {
                return OperationResult.Fail<int>(WrongNotificationIdError);
            }

            if (notification.UserId != userId)
            {
                return OperationResult.Fail<int>(DeleteNotificationError);
            }

            this.notificationRepo.Delete(notification);
            await this.notificationRepo.SaveChangesAsync();

            var result = await this.GetUserNotificationsCountAsync(userId);

            return OperationResult.Ok(result.Data);
        }

        public async Task<OperationResult<NotificationListViewModel>> GetUserNotificationsAsync(string userId)
        {
            var notifications = await this.notificationRepo
                .AllAsNoTracking()
                .Where(x => x.UserId == userId)
                .OrderByDescending(n => n.CreatedOn)
                .To<NotificationViewModel>()
                .ToListAsync();

            var model = new NotificationListViewModel
            {
                Notifications = notifications,
            };

            return OperationResult.Ok(model);
        }

        public async Task<OperationResult<int>> GetUserNotificationsCountAsync(string userId)
        {
            var count = await this.notificationRepo
                         .AllAsNoTracking()
                         .CountAsync(n => !n.IsRead && n.UserId == userId);

            return OperationResult.Ok(count);
        }

        public async Task<OperationResult> MarkOldNotificationsAsDeletedAsync()
        {
            var cutoffTime = DateTime.UtcNow.AddDays(-3);

            var notifications = await this.notificationRepo
                .All()
                .Where(n => !n.IsDeleted && n.CreatedOn <= cutoffTime)
                .ToListAsync();

            foreach (var notification in notifications)
            {
                this.notificationRepo.Delete(notification);
            }

            await this.notificationRepo.SaveChangesAsync();

            return OperationResult.Ok();
        }

        public async Task<OperationResult> MarkUnreadUserNotificationsAsReadAsync(string userId)
        {
            var userNotifications = await this.notificationRepo
                .All()
                .Where(n => !n.IsRead && n.UserId == userId)
                .ToListAsync();

            foreach (var notification in userNotifications)
            {
                notification.IsRead = true;
                this.notificationRepo.Update(notification);
            }

            if (userNotifications.Count > 0)
            {
                await this.notificationRepo.SaveChangesAsync();
            }

            return OperationResult.Ok("Successfully marked notifications as read!");
        }
    }
}
