namespace Bookworm.Data.Seeding.Seeders
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    using Bookworm.Data.Models;

    using Microsoft.AspNetCore.Identity;
    using Microsoft.Extensions.DependencyInjection;

    using static Bookworm.Common.Constants.GlobalConstants;

    internal class RolesSeeder : ISeeder
    {
        public async Task SeedAsync(
            ApplicationDbContext dbContext,
            IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider
                .GetRequiredService<RoleManager<ApplicationRole>>();

            await SeedRoleAsync(roleManager, UserRoleName);
            await SeedRoleAsync(roleManager, AdministratorRoleName);
        }

        private static async Task SeedRoleAsync(
            RoleManager<ApplicationRole> roleManager,
            string roleName)
        {
            var role = await roleManager.FindByNameAsync(roleName);

            if (role == null)
            {
                var newRole = new ApplicationRole(roleName);

                var result = await roleManager.CreateAsync(newRole);

                if (!result.Succeeded)
                {
                    var exceptionMessage = string.Join(
                        Environment.NewLine,
                        result.Errors.Select(e => e.Description));

                    throw new Exception(exceptionMessage);
                }
            }
        }
    }
}
