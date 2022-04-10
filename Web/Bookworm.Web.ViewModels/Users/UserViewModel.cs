namespace Bookworm.Web.ViewModels.Users
{
    using System.Collections.Generic;

    using Microsoft.AspNetCore.Http;

    public class UserViewModel
    {
        public string Id { get; set; }

        public string UserName { get; set; }

        public string ProfilePictureUrl { get; set; }

        public IFormFile ProfilePictureFile { get; set; }

        public string Email { get; set; }

        public int Points { get; set; }

        public IList<string> Roles { get; set; }
    }
}
