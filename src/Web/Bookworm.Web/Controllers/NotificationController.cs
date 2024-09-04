namespace Bookworm.Web.Controllers
{
    using System.Threading.Tasks;

    using Bookworm.Services.Data.Contracts;
    using Bookworm.Web.Extensions;
    using Microsoft.AspNetCore.Mvc;

    [Route("Notifications")]
    public class NotificationController : BaseController
    {
        private readonly INotificationService notificationService;

        public NotificationController(INotificationService notificationService)
        {
            this.notificationService = notificationService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var userId = this.User.GetId();

            var result = await this.notificationService.GetUserNotificationsAsync(userId);

            return this.View(result.Data);
        }
    }
}
