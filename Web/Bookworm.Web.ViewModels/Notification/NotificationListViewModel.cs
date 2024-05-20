namespace Bookworm.Web.ViewModels.Notification
{
    using System.Collections.Generic;

    public class NotificationListViewModel
    {
        public IEnumerable<NotificationViewModel> Notifications { get; set; }
    }
}
