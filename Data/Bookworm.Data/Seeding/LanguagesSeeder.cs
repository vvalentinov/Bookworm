namespace Bookworm.Data.Seeding
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Bookworm.Data.Models;

    public class LanguagesSeeder : ISeeder
    {
        private readonly List<Language> languages;

        public LanguagesSeeder()
        {
            this.languages = new List<Language>()
            {
                new Language() { Name = "Albanian" },
                new Language() { Name = "Arabic" },
                new Language() { Name = "Armenian" },
                new Language() { Name = "Belarusian" },
                new Language() { Name = "Bengali" },
                new Language() { Name = "Bulgarian" },
                new Language() { Name = "Catalan" },
                new Language() { Name = "Chinese" },
                new Language() { Name = "Croatian" },
                new Language() { Name = "Czech" },
                new Language() { Name = "Danish" },
                new Language() { Name = "Dutch" },
                new Language() { Name = "English" },
                new Language() { Name = "Estonian" },
                new Language() { Name = "Finnish" },
                new Language() { Name = "French" },
                new Language() { Name = "German" },
                new Language() { Name = "Greek" },
                new Language() { Name = "Hebrew" },
                new Language() { Name = "Hindi" },
                new Language() { Name = "Hungarian" },
                new Language() { Name = "Indonesian" },
                new Language() { Name = "Italian" },
                new Language() { Name = "Japanese" },
                new Language() { Name = "Korean" },
                new Language() { Name = "Latin" },
                new Language() { Name = "Mongolian" },
                new Language() { Name = "Persian" },
                new Language() { Name = "Polish" },
                new Language() { Name = "Portuguese" },
                new Language() { Name = "Romanian" },
                new Language() { Name = "Russian" },
                new Language() { Name = "Sanskrit" },
                new Language() { Name = "Serbian" },
                new Language() { Name = "Slovak" },
                new Language() { Name = "Slovenian" },
                new Language() { Name = "Spanish" },
                new Language() { Name = "Swedish" },
                new Language() { Name = "Turkish" },
                new Language() { Name = "Ukrainian" },
                new Language() { Name = "Vietnamese" },
            };
        }

        public async Task SeedAsync(ApplicationDbContext dbContext, IServiceProvider serviceProvider)
        {
            if (dbContext.Languages.Any())
            {
                return;
            }

            await dbContext.Languages.AddRangeAsync(this.languages);
            await dbContext.SaveChangesAsync();
        }
    }
}
