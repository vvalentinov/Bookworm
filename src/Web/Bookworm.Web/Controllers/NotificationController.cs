namespace Bookworm.Web.Controllers
{
    using System.Threading.Tasks;

    using Bookworm.Data.Models;
    using Bookworm.Services.Data.Contracts;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;

    [Route("Notifications")]
    public class NotificationController : BaseController
    {
        private readonly INotificationService notificationService;
        private readonly UserManager<ApplicationUser> userManager;

        public NotificationController(
            INotificationService notificationService,
            UserManager<ApplicationUser> userManager)
        {
            this.userManager = userManager;
            this.notificationService = notificationService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var userId = this.userManager.GetUserId(this.User);
            var model = await this.notificationService.GetUserNotificationsAsync(userId);
            return this.View(model);
        }
    }
}
