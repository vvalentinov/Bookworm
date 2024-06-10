namespace Bookworm.Web.Areas.Admin.Controllers
{
    using System;
    using System.Threading.Tasks;

    using Bookworm.Data.Models;
    using Bookworm.Services.Data.Contracts;
    using Bookworm.Web.ViewModels.Users;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;

    using static Bookworm.Common.Constants.TempDataMessageConstant;

    public class UserController : BaseController
    {
        private readonly IUsersService usersService;
        private readonly RoleManager<ApplicationRole> roleManager;
        private readonly UserManager<ApplicationUser> userManager;

        public UserController(
            IUsersService usersService,
            RoleManager<ApplicationRole> roleManager,
            UserManager<ApplicationUser> userManager)
        {
            this.userManager = userManager;
            this.roleManager = roleManager;
            this.usersService = usersService;
        }

        [HttpGet]
        public async Task<IActionResult> Index() => this.View(await this.usersService.GetUsersAsync());

        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            try
            {
                var user = await this.usersService.GetUserWithIdAsync(id);
                var roles = await this.roleManager.Roles.ToListAsync();

                this.ViewData["Roles"] = roles;

                var model = new UserViewModel
                {
                    Id = user.Id,
                    Email = user.Email,
                    UserName = user.UserName,
                    Roles = await this.userManager.GetRolesAsync(user),
                };

                return this.View(model);
            }
            catch (Exception ex)
            {
                this.TempData[ErrorMessage] = ex.Message;
                return this.RedirectToAction("Index", "Home");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Edit(UserViewModel model)
        {
            try
            {
                await this.usersService.EditUserAsync(model.Id, model.UserName, model.Roles);
                return this.RedirectToAction(nameof(this.Index));
            }
            catch (Exception ex)
            {
                this.TempData[ErrorMessage] = ex.Message;
                return this.RedirectToAction("Index", "Home");
            }
        }
    }
}
