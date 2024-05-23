namespace Bookworm.Services.Data.Models
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    using Bookworm.Data.Common.Repositories;
    using Bookworm.Data.Models;
    using Bookworm.Services.Data.Contracts;
    using Bookworm.Services.Mapping;
    using Bookworm.Web.ViewModels.Notification;
    using Microsoft.EntityFrameworkCore;

    public class NotificationService : INotificationService
    {
        private readonly IDeletableEntityRepository<Notification> notificationRepo;

        public NotificationService(IDeletableEntityRepository<Notification> notificationRepo)
        {
            this.notificationRepo = notificationRepo;
        }

        public async Task AddNotificationAsync(string content, string userId)
        {
            var notification = new Notification { UserId = userId, Content = content };
            await this.notificationRepo.AddAsync(notification);
            await this.notificationRepo.SaveChangesAsync();
        }

        public async Task<int> DeleteUserNotificationAsync(string userId, int notificationId)
        {
            var notification = await this.notificationRepo.All()
                .FirstOrDefaultAsync(n => n.Id == notificationId) ??
                throw new InvalidOperationException("No notification found with given id!");

            if (notification.UserId != userId)
            {
                throw new InvalidOperationException("Only the notification's owner can delete it!");
            }

            this.notificationRepo.Delete(notification);
            await this.notificationRepo.SaveChangesAsync();

            return await this.GetUserNotificationsCountAsync(userId);
        }

        public async Task<NotificationListViewModel> GetUserNotificationsAsync(string userId)
        {
            var notifications = await this.notificationRepo
                .AllAsNoTracking()
                .Where(x => x.UserId == userId)
                .OrderByDescending(n => n.CreatedOn)
                .To<NotificationViewModel>()
                .ToListAsync();

            return new NotificationListViewModel { Notifications = notifications };
        }

        public async Task<int> GetUserNotificationsCountAsync(string userId)
            => await this.notificationRepo.AllAsNoTracking().CountAsync(n => n.UserId == userId && !n.IsRead);

        public async Task MarkOldNotificationsAsDeletedAsync()
        {
            var cutoffTime = DateTime.UtcNow.AddDays(-3);

            var notifications = await this.notificationRepo.AllAsNoTracking()
                .Where(n => !n.IsDeleted && n.CreatedOn <= cutoffTime).ToListAsync();

            foreach (var notification in notifications)
            {
                this.notificationRepo.Delete(notification);
            }

            await this.notificationRepo.SaveChangesAsync();
        }

        public async Task MarkUnreadUserNotificationsAsReadAsync(string userId)
        {
            var userNotifications = await this.notificationRepo.All()
                .Where(n => n.UserId == userId && !n.IsRead).ToListAsync();

            foreach (var notification in userNotifications)
            {
                notification.IsRead = true;
                this.notificationRepo.Update(notification);
            }

            if (userNotifications.Count > 0)
            {
                await this.notificationRepo.SaveChangesAsync();
            }
        }
    }
}
