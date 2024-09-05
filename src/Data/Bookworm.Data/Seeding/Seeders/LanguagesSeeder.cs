namespace Bookworm.Data.Seeding.Seeders
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Bookworm.Data.Models;
    using Microsoft.EntityFrameworkCore;

    public class LanguagesSeeder : ISeeder
    {
        private readonly List<Language> languages;

        public LanguagesSeeder()
        {
            this.languages =
            [
                new() { Name = "Albanian" },
                new() { Name = "Arabic" },
                new() { Name = "Armenian" },
                new() { Name = "Belarusian" },
                new() { Name = "Bengali" },
                new() { Name = "Bulgarian" },
                new() { Name = "Catalan" },
                new() { Name = "Chinese" },
                new() { Name = "Croatian" },
                new() { Name = "Czech" },
                new() { Name = "Danish" },
                new() { Name = "Dutch" },
                new() { Name = "English" },
                new() { Name = "Estonian" },
                new() { Name = "Finnish" },
                new() { Name = "French" },
                new() { Name = "German" },
                new() { Name = "Greek" },
                new() { Name = "Hebrew" },
                new() { Name = "Hindi" },
                new() { Name = "Hungarian" },
                new() { Name = "Indonesian" },
                new() { Name = "Italian" },
                new() { Name = "Japanese" },
                new() { Name = "Korean" },
                new() { Name = "Latin" },
                new() { Name = "Mongolian" },
                new() { Name = "Persian" },
                new() { Name = "Polish" },
                new() { Name = "Portuguese" },
                new() { Name = "Romanian" },
                new() { Name = "Russian" },
                new() { Name = "Sanskrit" },
                new() { Name = "Serbian" },
                new() { Name = "Slovak" },
                new() { Name = "Slovenian" },
                new() { Name = "Spanish" },
                new() { Name = "Swedish" },
                new() { Name = "Turkish" },
                new() { Name = "Ukrainian" },
                new() { Name = "Vietnamese" },
            ];
        }

        public async Task SeedAsync(
            ApplicationDbContext dbContext,
            IServiceProvider serviceProvider)
        {
            if (await dbContext.Languages.AnyAsync())
            {
                return;
            }

            await dbContext
                .Languages
                .AddRangeAsync(this.languages);

            await dbContext.SaveChangesAsync();
        }
    }
}
