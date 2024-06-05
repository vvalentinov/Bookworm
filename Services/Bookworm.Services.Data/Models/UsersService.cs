namespace Bookworm.Services.Data.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Bookworm.Data.Models;
    using Bookworm.Services.Data.Contracts;
    using Bookworm.Web.ViewModels.Users;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.EntityFrameworkCore;

    using static Bookworm.Common.Constants.ErrorMessagesConstants.IdentityErrorMessagesConstants;
    using static Bookworm.Common.Constants.GlobalConstants;

    public class UsersService : IUsersService
    {
        private readonly UserManager<ApplicationUser> userManager;

        public UsersService(UserManager<ApplicationUser> userManager)
        {
            this.userManager = userManager;
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
            return this.userManager.Users
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
                throw new InvalidOperationException(UserWrongIdError);

        public async Task IncreaseUserPointsAsync(string userId, byte points)
        {
            var user = await this.GetUserWithIdAsync(userId);
            user.Points += points;
            await this.userManager.UpdateAsync(user);
        }

        public async Task ReduceUserPointsAsync(string userId, byte points)
        {
            var user = await this.GetUserWithIdAsync(userId);
            user.Points = user.Points - points < 0 ? 0 : user.Points - points;
            await this.userManager.UpdateAsync(user);
        }

        public async Task<bool> IsUserAdminAsync(string userId)
            => await this.userManager.IsInRoleAsync(await this.GetUserWithIdAsync(userId), AdministratorRoleName);

        public async Task IncreaseUserDailyDownloadsCountAsync(ApplicationUser user)
        {
            if (user.DailyDownloadsCount < this.GetUserDailyMaxDownloadsCount(user.Points))
            {
                user.DailyDownloadsCount++;
                await this.userManager.UpdateAsync(user);
            }
        }

        public byte GetUserDailyMaxDownloadsCount(int userPoints)
        {
            return userPoints switch
            {
                < 100 => 10,
                >= 100 and < 200 => 15,
                >= 200 and < 300 => 20,
                >= 300 and < 400 => 25,
                _ => 30,
            };
        }

        public async Task<string> GetUserNameByIdAsync(string userId)
        {
            var user = await this.userManager
                .Users
                .FirstOrDefaultAsync(u => u.Id == userId) ??
                throw new InvalidOperationException(UserWrongIdError);

            return user.UserName;
        }
    }
}
