namespace Bookworm.Web.ViewModels.Notification
{
    using Bookworm.Data.Models;
    using Bookworm.Services.Mapping;

    public class NotificationViewModel : IMapFrom<Notification>
    {
        public int Id { get; set; }

        public string Content { get; set; }
    }
}
