namespace Bookworm.Services.Data.Models
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Azure.Storage.Blobs;
    using Bookworm.Data.Common.Repositories;
    using Bookworm.Data.Models;
    using Bookworm.Services.Data.Contracts;
    using Bookworm.Web.ViewModels.Users;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.Extensions.Configuration;

    public class UsersService : IUsersService
    {
        private readonly IDeletableEntityRepository<ApplicationUser> usersRepository;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IConfiguration configuration;
        private readonly IBlobService blobService;

        public UsersService(
            IDeletableEntityRepository<ApplicationUser> usersRepository,
            UserManager<ApplicationUser> userManager,
            IConfiguration configuration,
            IBlobService blobService)
        {
            this.usersRepository = usersRepository;
            this.userManager = userManager;
            this.configuration = configuration;
            this.blobService = blobService;
        }

        public async Task EditUser(string userId, string username, IFormFile pictureFile)
        {
            ApplicationUser user = this.usersRepository.All().First(x => x.Id == userId);
            if (username != null && user.UserName != username)
            {
                user.UserName = username;
            }

            string anonymousPictureUrl = this.configuration.GetValue<string>("AnonymousProfilePictureUrl");
            if (pictureFile != null)
            {
                if (user.ProfilePictureUrl != null && user.ProfilePictureUrl != anonymousPictureUrl)
                {
                    BlobClient blob = this.blobService.GetBlobClient(user.ProfilePictureUrl);
                    await this.blobService.DeleteBlobAsync(blob.Name);
                }

                await this.blobService.UploadBlobAsync(pictureFile);
                string pictureUrl = this.blobService.GetBlobAbsoluteUri(pictureFile.FileName);
                user.ProfilePictureUrl = pictureUrl;
            }
            else
            {
                user.ProfilePictureUrl = anonymousPictureUrl;
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
                ProfilePictureUrl = user.ProfilePictureUrl,
                Roles = roles,
            };
        }

        public ApplicationUser GetUserWithId(string id)
        {
            return this.usersRepository
                .All()
                .FirstOrDefault(x => x.Id == id);
        }
    }
}
