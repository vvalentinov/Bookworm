namespace Bookworm.Web.Areas.Admin.Controllers
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Bookworm.Data.Common.Repositories;
    using Bookworm.Data.Models;
    using Bookworm.Services.Data.Contracts;
    using Bookworm.Web.ViewModels.Users;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;

    public class UserController : BaseController
    {
        private readonly IUsersService usersService;
        private readonly RoleManager<ApplicationRole> roleManager;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IDeletableEntityRepository<ApplicationRole> rolesRepository;

        public UserController(
            IUsersService usersService,
            RoleManager<ApplicationRole> roleManager,
            UserManager<ApplicationUser> userManager,
            IDeletableEntityRepository<ApplicationRole> rolesRepository)
        {
            this.usersService = usersService;
            this.roleManager = roleManager;
            this.userManager = userManager;
            this.rolesRepository = rolesRepository;
        }

        public IActionResult Index()
        {
            var model = this.usersService.GetUsers();
            return this.View(model);
        }

        public async Task<IActionResult> Roles(string userId)
        {
            ApplicationUser user = this.usersService.GetUserWithId(userId);
            IList<string> userRoles = await this.userManager.GetRolesAsync(user);

            IEnumerable<SelectListItem> roles = this.roleManager
                .Roles
                .ToList()
                .Select(x => new SelectListItem()
                {
                    Text = x.Name,
                    Value = x.Name,
                    Selected = this.userManager.IsInRoleAsync(user, x.Name).Result,
                });

            UserRolesViewModel model = new UserRolesViewModel()
            {
                UserId = userId,
                Username = user.UserName,
                UserRoles = userRoles,
            };

            this.ViewBag.RoleItems = this.roleManager.Roles
                .ToList()
                .Select(r => new SelectListItem()
                {
                    Text = r.Name,
                    Value = r.Name,
                    Selected = this.userManager.IsInRoleAsync(user, r.Name).Result,
                }).ToList();

            return this.View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Roles(UserRolesViewModel model)
        {
            ApplicationUser user = this.usersService.GetUserWithId(model.UserId);
            IList<string> userRoles = await this.userManager.GetRolesAsync(user);
            await this.userManager.RemoveFromRolesAsync(user, userRoles);

            if (model.Roles.Any())
            {
                await this.userManager.AddToRolesAsync(user, model.Roles);
            }

            return this.RedirectToAction(nameof(this.Index));
        }

        public async Task<IActionResult> Edit(string id)
        {
            UserViewModel model = await this.usersService.GetUserModelWithId(id);
            return this.View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(UserViewModel model)
        {
            await this.usersService.EditUser(model.Id, model.UserName, model.ProfilePictureFile);
            return this.RedirectToAction(nameof(this.Edit), new { model.Id });
        }
    }
}
