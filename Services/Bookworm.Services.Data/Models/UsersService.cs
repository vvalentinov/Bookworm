namespace Bookworm.Services.Data.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Bookworm.Data.Common.Repositories;
    using Bookworm.Data.Models;
    using Bookworm.Services.Data.Contracts;
    using Bookworm.Web.ViewModels.Users;
    using Microsoft.AspNetCore.Identity;

    using static Bookworm.Common.GlobalConstants;

    public class UsersService : IUsersService
    {
        private readonly IDeletableEntityRepository<Book> bookRepository;
        private readonly IRepository<Quote> quoteRepository;
        private readonly UserManager<ApplicationUser> userManager;

        public UsersService(
            IDeletableEntityRepository<Book> bookRepository,
            IRepository<Quote> quoteRepository,
            UserManager<ApplicationUser> userManager)
        {
            this.bookRepository = bookRepository;
            this.quoteRepository = quoteRepository;
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

        public IEnumerable<UserStatisticsViewModel> GetUsersStatistics()
        {
            return this.userManager
                .Users
                .Select(u => new UserStatisticsViewModel
                {
                    Id = u.Id,
                    UserName = u.UserName,
                    UploadedBooks = this.bookRepository
                        .AllAsNoTracking()
                        .Where(b => b.UserId == u.Id && b.IsApproved == true)
                        .Count(),
                    UploadedQuotes = this.quoteRepository
                        .AllAsNoTracking()
                        .Where(q => q.UserId == u.Id && q.IsApproved == true)
                        .Count(),
                }).ToList();
        }

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
    }
}
