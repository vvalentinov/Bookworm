namespace Bookworm.Data.Seeding
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    using Bookworm.Data.Models;

    public class LanguagesSeeder : ISeeder
    {
        public async Task SeedAsync(ApplicationDbContext dbContext, IServiceProvider serviceProvider)
        {
            if (dbContext.Languages.Any())
            {
                return;
            }

            await dbContext.Languages.AddAsync(new Language { Name = "Albanian" });
            await dbContext.Languages.AddAsync(new Language { Name = "Arabic" });
            await dbContext.Languages.AddAsync(new Language { Name = "Armenian" });
            await dbContext.Languages.AddAsync(new Language { Name = "Belarusian" });
            await dbContext.Languages.AddAsync(new Language { Name = "Bengali" });
            await dbContext.Languages.AddAsync(new Language { Name = "Bulgarian" });
            await dbContext.Languages.AddAsync(new Language { Name = "Catalan" });
            await dbContext.Languages.AddAsync(new Language { Name = "Chinese" });
            await dbContext.Languages.AddAsync(new Language { Name = "Croatian" });
            await dbContext.Languages.AddAsync(new Language { Name = "Czech" });
            await dbContext.Languages.AddAsync(new Language { Name = "Danish" });
            await dbContext.Languages.AddAsync(new Language { Name = "Dutch" });
            await dbContext.Languages.AddAsync(new Language { Name = "English" });
            await dbContext.Languages.AddAsync(new Language { Name = "Estonian" });
            await dbContext.Languages.AddAsync(new Language { Name = "Finnish" });
            await dbContext.Languages.AddAsync(new Language { Name = "French" });
            await dbContext.Languages.AddAsync(new Language { Name = "German" });
            await dbContext.Languages.AddAsync(new Language { Name = "Greek" });
            await dbContext.Languages.AddAsync(new Language { Name = "Hebrew" });
            await dbContext.Languages.AddAsync(new Language { Name = "Hindi" });
            await dbContext.Languages.AddAsync(new Language { Name = "Hungarian" });
            await dbContext.Languages.AddAsync(new Language { Name = "Indonesian" });
            await dbContext.Languages.AddAsync(new Language { Name = "Italian" });
            await dbContext.Languages.AddAsync(new Language { Name = "Japanese" });
            await dbContext.Languages.AddAsync(new Language { Name = "Korean" });
            await dbContext.Languages.AddAsync(new Language { Name = "Latin" });
            await dbContext.Languages.AddAsync(new Language { Name = "Mongolian" });
            await dbContext.Languages.AddAsync(new Language { Name = "Persian" });
            await dbContext.Languages.AddAsync(new Language { Name = "Polish" });
            await dbContext.Languages.AddAsync(new Language { Name = "Portuguese" });
            await dbContext.Languages.AddAsync(new Language { Name = "Romanian" });
            await dbContext.Languages.AddAsync(new Language { Name = "Russian" });
            await dbContext.Languages.AddAsync(new Language { Name = "Sanskrit" });
            await dbContext.Languages.AddAsync(new Language { Name = "Serbian" });
            await dbContext.Languages.AddAsync(new Language { Name = "Slovak" });
            await dbContext.Languages.AddAsync(new Language { Name = "Slovenian" });
            await dbContext.Languages.AddAsync(new Language { Name = "Spanish" });
            await dbContext.Languages.AddAsync(new Language { Name = "Swedish" });
            await dbContext.Languages.AddAsync(new Language { Name = "Turkish" });
            await dbContext.Languages.AddAsync(new Language { Name = "Ukrainian" });
            await dbContext.Languages.AddAsync(new Language { Name = "Vietnamese" });
        }
    }
}
