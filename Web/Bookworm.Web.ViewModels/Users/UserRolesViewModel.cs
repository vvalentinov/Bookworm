namespace Bookworm.Web.ViewModels.Users
{
    using System.Collections.Generic;

    public class UserRolesViewModel
    {
        public string UserId { get; set; }

        public string Username { get; set; }

        public IList<string> UserRoles { get; set; }

        public IEnumerable<string> Roles { get; set; }
    }
}
