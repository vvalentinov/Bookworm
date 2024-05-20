namespace Bookworm.Web.Controllers.Api
{
    using System;
    using System.Threading.Tasks;

    using Bookworm.Data.Models;
    using Bookworm.Services.Data.Contracts;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;

    public class ApiNotificationController : ApiBaseController
    {
        private readonly INotificationService notificationService;
        private readonly UserManager<ApplicationUser> userManager;

        public ApiNotificationController(
            INotificationService notificationService,
            UserManager<ApplicationUser> userManager)
        {
            this.userManager = userManager;
            this.notificationService = notificationService;
        }

        [HttpGet(nameof(GetUserNotificationsCount))]
        public async Task<int> GetUserNotificationsCount()
        {
            var userId = this.userManager.GetUserId(this.User);
            return await this.notificationService.GetUserNotificationsCountAsync(userId);
        }

        [HttpDelete(nameof(DeleteNotification))]
        public async Task<ActionResult<int>> DeleteNotification(int id)
        {
            try
            {
                var userId = this.userManager.GetUserId(this.User);
                return await this.notificationService.DeleteUserNotificationAsync(userId, id);
            }
            catch (Exception ex)
            {
                return this.BadRequest(ex.Message);
            }
        }
    }
}
