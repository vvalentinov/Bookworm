namespace Bookworm.Web.Controllers.Api
{
    using System.Threading.Tasks;

    using Bookworm.Services.Data.Contracts;
    using Bookworm.Web.Extensions;
    using Microsoft.AspNetCore.Mvc;

    public class ApiNotificationController : ApiBaseController
    {
        private readonly INotificationService service;

        public ApiNotificationController(INotificationService service)
        {
            this.service = service;
        }

        [HttpGet(nameof(GetUserNotificationsCount))]
        public async Task<int> GetUserNotificationsCount()
        {
            var userId = this.User.GetId();

            var result = await this.service
                .GetUserNotificationsCountAsync(userId);

            return result.Data;
        }

        [HttpPut(nameof(MarkUserNotificationsAsRead))]
        public async Task<IActionResult> MarkUserNotificationsAsRead()
        {
            var userId = this.User.GetId();

            var result = await this.service
                .MarkUnreadUserNotificationsAsReadAsync(userId);

            return this.Ok(result.SuccessMessage);
        }

        [HttpDelete(nameof(DeleteNotification))]
        public async Task<ActionResult<int>> DeleteNotification(int id)
        {
            var userId = this.User.GetId();

            var result = await this.service
                .DeleteUserNotificationAsync(userId, id);

            if (result.IsSuccess)
            {
                return this.Ok(result.Data);
            }

            return this.BadRequest(new { error = result.ErrorMessage });
        }
    }
}
