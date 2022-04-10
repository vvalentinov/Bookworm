namespace Bookworm.Web.Controllers
{
    using System.Collections.Generic;

    using Bookworm.Services.Data.Contracts;
    using Bookworm.Web.ViewModels.Users;
    using Microsoft.AspNetCore.Mvc;

    public class UserController : BaseController
    {
        private readonly IUsersService usersService;

        public UserController(IUsersService usersService)
        {
            this.usersService = usersService;
        }

        public IActionResult Statistics()
        {
            IEnumerable<UserStatisticsViewModel> users = this.usersService.GetUsersStatistics();
            return this.View(users);
        }
    }
}
