namespace Bookworm.Services.Data.Tests
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Bookworm.Data;
    using Bookworm.Data.Models;
    using Bookworm.Services.Data.Models;
    using Bookworm.Services.Data.Tests.Shared;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.EntityFrameworkCore;
    using Moq;
    using Xunit;

    public class UsersServiceTests : IClassFixture<DbContextFixture>
    {
        private readonly ApplicationDbContext dbContext;

        public UsersServiceTests(DbContextFixture dbContextFixture)
        {
            this.dbContext = dbContextFixture.DbContext;
        }

        [Fact]
        public async Task EditUserShouldWorkCorrectly()
        {
            var service = this.GetUsersService();

            await service.EditUserAsync("a84ea5dc-a89e-442f-8e53-c874675bb114", "New Username", new List<string>() { "Administrator" });

            var user = this.dbContext.Users.First(x => x.Id == "a84ea5dc-a89e-442f-8e53-c874675bb114");

            Assert.Equal("New Username", user.UserName);
            Assert.Contains("fcea6658-70d4-4dac-a582-bb836b493323", user.Roles.Select(x => x.RoleId));
        }

        [Fact]
        public async Task GetUsersShouldWorkCorrectly()
        {
            var service = this.GetUsersService();

            var users = await service.GetUsersAsync();

            Assert.Equal(4, users.Count());
        }

        [Theory]
        [InlineData(100)]
        [InlineData(213)]
        [InlineData(399)]
        [InlineData(400)]
        public void GetUserDailyMaxDownloadsCountShouldWorkCorrectly(int userPoints)
        {
            var service = this.GetUsersService();

            var maxCount = service.GetUserDailyMaxDownloadsCount(userPoints);

            switch (userPoints)
            {
                case 100:
                    Assert.Equal(15, maxCount);
                    break;
                case 213:
                    Assert.Equal(20, maxCount);
                    break;
                case 399:
                    Assert.Equal(25, maxCount);
                    break;
                case 400:
                    Assert.Equal(30, maxCount);
                    break;
            }
        }

        private UsersService GetUsersService() => new(this.GetUserManager());

        private UserManager<ApplicationUser> GetUserManager()
        {
            var store = new Mock<IUserStore<ApplicationUser>>();

            var users = this.dbContext.Users.AsQueryable();
            var allRoles = this.dbContext.Roles.AsNoTracking().ToList();

            var mockUserDbSet = new Mock<DbSet<ApplicationUser>>();
            mockUserDbSet.As<IQueryable<ApplicationUser>>().Setup(m => m.Provider).Returns(users.Provider);
            mockUserDbSet.As<IQueryable<ApplicationUser>>().Setup(m => m.Expression).Returns(users.Expression);
            mockUserDbSet.As<IQueryable<ApplicationUser>>().Setup(m => m.ElementType).Returns(users.ElementType);
            mockUserDbSet.As<IQueryable<ApplicationUser>>().Setup(m => m.GetEnumerator()).Returns(users.GetEnumerator());

            var userManagerMock = new Mock<UserManager<ApplicationUser>>(
                store.Object, null, null, null, null, null, null, null, null);

            userManagerMock.Setup(x => x.Users).Returns(mockUserDbSet.Object);

            userManagerMock
                .Setup(x => x.FindByIdAsync(It.IsAny<string>()))
                .ReturnsAsync((string userId) => users.FirstOrDefault(u => u.Id == userId));

            userManagerMock
                .Setup(x => x.GetRolesAsync(It.IsAny<ApplicationUser>()))
                .ReturnsAsync((ApplicationUser user) =>
                {
                    var roleIds = this.dbContext.UserRoles.Where(x => x.UserId == user.Id).Select(x => x.RoleId).ToList();
                    var roles = this.dbContext.Roles.Where(x => roleIds.Contains(x.Id)).Select(x => x.Name).ToList();
                    return roles;
                });

            userManagerMock
                .Setup(x => x.RemoveFromRolesAsync(It.IsAny<ApplicationUser>(), It.IsAny<IEnumerable<string>>()))
                .ReturnsAsync((ApplicationUser user, IEnumerable<string> roles) =>
                {
                    foreach (var roleName in roles)
                    {
                        var role = this.dbContext.Roles.First(x => x.Name == roleName);

                        var userRole = this.dbContext.UserRoles.First(x => x.UserId == user.Id && x.RoleId == role.Id);

                        this.dbContext.UserRoles.Remove(userRole);
                    }

                    this.dbContext.SaveChanges();

                    return IdentityResult.Success;
                });

            userManagerMock
                .Setup(x => x.UpdateAsync(It.IsAny<ApplicationUser>()))
                .ReturnsAsync((ApplicationUser userInput) =>
                {
                    var user = this.dbContext.Users.First(x => x.Id == userInput.Id);

                    user.UserName = userInput.UserName;

                    user.Roles = userInput.Roles;

                    this.dbContext.SaveChanges();

                    return IdentityResult.Success;
                });

            userManagerMock
                .Setup(x => x.IsInRoleAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
                .ReturnsAsync((ApplicationUser user, string role) =>
                {
                    var roleIds = this.dbContext.UserRoles.Where(x => x.UserId == user.Id).Select(x => x.RoleId).ToList();

                    var roleNames = allRoles.Where(x => roleIds.Contains(x.Id)).Select(x => x.Name);

                    return roleNames.Contains(role);
                });

            userManagerMock
                .Setup(x => x.AddToRoleAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
                .ReturnsAsync((ApplicationUser user, string roleInput) =>
                {
                    var role = allRoles.FirstOrDefault(x => x.Name == roleInput);

                    this.dbContext.UserRoles.Add(new IdentityUserRole<string>() { RoleId = role.Id, UserId = user.Id });

                    this.dbContext.SaveChanges();

                    return IdentityResult.Success;
                });

            return userManagerMock.Object;
        }
    }
}
