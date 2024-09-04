namespace Bookworm.Services.Data.Contracts
{
    using System.Threading.Tasks;

    using Bookworm.Common;
    using Bookworm.Web.ViewModels.Notification;

    public interface INotificationService
    {
        Task<OperationResult> AddNotificationAsync(
            string content,
            string userId);

        Task<OperationResult<NotificationListViewModel>> GetUserNotificationsAsync(string userId);

        Task<OperationResult> MarkOldNotificationsAsDeletedAsync();

        Task<OperationResult<int>> GetUserNotificationsCountAsync(string userId);

        Task<OperationResult<int>> DeleteUserNotificationAsync(
            string userId,
            int notificationId);

        Task<OperationResult> MarkUnreadUserNotificationsAsReadAsync(string userId);
    }
}
