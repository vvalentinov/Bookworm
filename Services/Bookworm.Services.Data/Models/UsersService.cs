namespace Bookworm.Services.Data.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Bookworm.Data;
    using Bookworm.Data.Models;
    using Bookworm.Services.Data.Contracts;
    using Bookworm.Web.ViewModels.Users;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.EntityFrameworkCore;

    using static Bookworm.Common.Constants.DataConstants.ApplicationUser;
    using static Bookworm.Common.Constants.GlobalConstants;

    public class UsersService : IUsersService
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly ApplicationDbContext dbContext;

        public UsersService(
            UserManager<ApplicationUser> userManager,
            ApplicationDbContext dbContext)
        {
            this.userManager = userManager;
            this.dbContext = dbContext;
        }

        public async Task EditUser(string userId, string username)
        {
            var user = await this.userManager.FindByIdAsync(userId);
            if (username != null && user.UserName != username)
            {
                user.UserName = username;
            }

            await this.userManager.UpdateAsync(user);
        }

        public IEnumerable<UsersListViewModel> GetUsers()
        {
            return this.userManager
                .Users
                .Select(x => new UsersListViewModel
                {
                    Id = x.Id,
                    Email = x.Email,
                    Username = x.UserName,
                }).ToList();
        }

        public async Task<UserViewModel> GetUserModelWithId(string id)
        {
            var user = await this.userManager.FindByIdAsync(id);
            var roles = await this.userManager.GetRolesAsync(user);

            return new UserViewModel
            {
                Id = id,
                UserName = user.UserName,
                Email = user.Email,
                Roles = roles,
            };
        }

        public async Task<ApplicationUser> GetUserWithIdAsync(string userId)
            => await this.userManager.FindByIdAsync(userId) ??
                throw new InvalidOperationException("No user with given id found!");

        public async Task ReduceUserPointsAsync(string userId, byte points)
        {
            var user = await this.GetUserWithIdAsync(userId);
            user.Points = user.Points - points < 0 ? 0 : user.Points - points;
            await this.userManager.UpdateAsync(user);
        }

        public async Task IncreaseUserPointsAsync(string userId, byte points)
        {
            var user = await this.GetUserWithIdAsync(userId);
            user.Points += points;
            await this.userManager.UpdateAsync(user);
        }

        public async Task<bool> IsUserAdminAsync(string userId)
            => await this.userManager.IsInRoleAsync(
                await this.GetUserWithIdAsync(userId),
                AdministratorRoleName);

        public async Task IncreaseUserDailyDownloadsCountAsync(ApplicationUser user)
        {
            if (user.DailyDownloadsCount < UserMaxDailyBookDownloadsCount)
            {
                user.DailyDownloadsCount++;
                await this.userManager.UpdateAsync(user);
            }
        }

        public async Task ResetDailyDownloadsCountAsync()
            => await this.dbContext.Database
                    .ExecuteSqlRawAsync("UPDATE AspNetUsers SET DailyDownloadsCount = 0");
    }
}
