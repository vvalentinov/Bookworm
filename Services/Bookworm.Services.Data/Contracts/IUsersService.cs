namespace Bookworm.Services.Data.Contracts
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Bookworm.Data.Models;
    using Bookworm.Web.ViewModels.Users;

    public interface IUsersService
    {
        Task<bool> IsUserAdminAsync(string userId);

        IEnumerable<UsersListViewModel> GetUsers();

        Task<UserViewModel> GetUserModelWithId(string userId);

        Task<ApplicationUser> GetUserWithIdAsync(string userId);

        Task EditUser(string userId, string username);

        Task ReduceUserPointsAsync(string userId, byte points);

        Task IncreaseUserPointsAsync(string userId, byte points);

        Task IncreaseUserDailyDownloadsCountAsync(ApplicationUser user);

        byte GetUserDailyMaxDownloadsCount(int userPoints);

        Task<string> GetUserNameByIdAsync(string userId);
    }
}
