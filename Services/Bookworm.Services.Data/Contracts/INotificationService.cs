namespace Bookworm.Services.Data.Contracts
{
    using System.Threading.Tasks;

    using Bookworm.Web.ViewModels.Notification;

    public interface INotificationService
    {
        Task AddNotificationAsync(string content, string userId);

        Task<NotificationListViewModel> GetUserNotificationsAsync(string userId);

        Task MarkOldNotificationsAsDeletedAsync();

        Task<int> GetUserNotificationsCountAsync(string userId);

        Task<int> DeleteUserNotificationAsync(string userId, int notificationId);

        Task MarkUnreadUserNotificationsAsReadAsync(string userId);
    }
}
