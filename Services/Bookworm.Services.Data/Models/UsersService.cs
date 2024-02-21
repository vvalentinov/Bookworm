namespace Bookworm.Services.Data.Models
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Bookworm.Data.Common.Repositories;
    using Bookworm.Data.Models;
    using Bookworm.Services.Data.Contracts;
    using Bookworm.Web.ViewModels.Users;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.Extensions.Configuration;

    public class UsersService : IUsersService
    {
        private readonly IDeletableEntityRepository<ApplicationUser> usersRepository;
        private readonly IDeletableEntityRepository<Book> bookRepository;
        private readonly IRepository<Quote> quoteRepository;
        private readonly UserManager<ApplicationUser> userManager;

        public UsersService(
            IDeletableEntityRepository<ApplicationUser> usersRepository,
            IDeletableEntityRepository<Book> bookRepository,
            IRepository<Quote> quoteRepository,
            UserManager<ApplicationUser> userManager,
            IConfiguration configuration,
            IBlobService blobService)
        {
            this.usersRepository = usersRepository;
            this.bookRepository = bookRepository;
            this.quoteRepository = quoteRepository;
            this.userManager = userManager;
        }

        public async Task EditUser(string userId, string username)
        {
            ApplicationUser user = this.usersRepository.All().First(x => x.Id == userId);
            if (username != null && user.UserName != username)
            {
                user.UserName = username;
            }

            await this.userManager.UpdateAsync(user);
        }

        public IEnumerable<UsersListViewModel> GetUsers()
        {
            return this.usersRepository
                .AllAsNoTracking()
                .Select(x => new UsersListViewModel()
                {
                    Id = x.Id,
                    Email = x.Email,
                    Username = x.UserName,
                }).ToList();
        }

        public async Task<UserViewModel> GetUserModelWithId(string id)
        {
            ApplicationUser user = this.usersRepository
                .All()
                .First(x => x.Id == id);

            IList<string> roles = await this.userManager.GetRolesAsync(user);
            return new UserViewModel()
            {
                Id = id,
                UserName = user.UserName,
                Email = user.Email,
                Roles = roles,
            };
        }

        public ApplicationUser GetUserWithId(string id)
        {
            return this.usersRepository
                .AllAsNoTracking()
                .FirstOrDefault(x => x.Id == id);
        }

        public IEnumerable<UserStatisticsViewModel> GetUsersStatistics()
        {
            return this.usersRepository
                .AllAsNoTracking()
                .Select(x => new UserStatisticsViewModel()
                {
                    Id = x.Id,
                    UserName = x.UserName,
                    UploadedBooks = this.bookRepository.AllAsNoTracking().Where(b => b.UserId == x.Id && b.IsApproved == true).Count(),
                    UploadedQuotes = this.quoteRepository.AllAsNoTracking().Where(q => q.UserId == x.Id && q.IsApproved == true).Count(),
                }).ToList();
        }

        public async Task ReduceUserPointsAsync(ApplicationUser user, byte points)
        {
            user.Points -= points;
            await this.userManager.UpdateAsync(user);
        }

        public async Task IncreaseUserPointsAsync(ApplicationUser user, byte points)
        {
            user.Points += points;
            await this.userManager.UpdateAsync(user);
        }
    }
}
