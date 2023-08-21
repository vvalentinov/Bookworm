namespace Bookworm.Services.Data.Contracts
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Bookworm.Data.Models;
    using Bookworm.Web.ViewModels.Users;

    public interface IUsersService
    {
        IEnumerable<UsersListViewModel> GetUsers();

        Task<UserViewModel> GetUserModelWithId(string id);

        ApplicationUser GetUserWithId(string id);

        Task EditUser(string userId, string username);

        IEnumerable<UserStatisticsViewModel> GetUsersStatistics();
    }
}
