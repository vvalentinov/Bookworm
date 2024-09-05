namespace Bookworm.Data.Seeding.Seeders
{
    using System;
    using System.Threading.Tasks;

    using Bookworm.Data.Models;
    using Microsoft.EntityFrameworkCore;

    internal class SettingsSeeder : ISeeder
    {
        public async Task SeedAsync(
            ApplicationDbContext dbContext,
            IServiceProvider serviceProvider)
        {
            if (await dbContext.Settings.AnyAsync())
            {
                return;
            }

            await dbContext
                .Settings
                .AddAsync(new Setting { Name = "Setting1", Value = "value1" });
        }
    }
}
