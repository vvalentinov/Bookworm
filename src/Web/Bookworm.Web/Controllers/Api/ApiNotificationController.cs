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

        [HttpPut(nameof(MarkUserNotificationsAsRead))]
        public async Task<IActionResult> MarkUserNotificationsAsRead()
        {
            try
            {
                var userId = this.userManager.GetUserId(this.User);
                await this.notificationService.MarkUnreadUserNotificationsAsReadAsync(userId);
                return this.Ok("Successfully marked notifications as read!");
            }
            catch (Exception ex)
            {
                return this.BadRequest(ex.Message);
            }
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
                return this.BadRequest(new { error = ex.Message });
            }
        }
    }
}
