namespace Bookworm.Data.Seeding
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    using Bookworm.Data.Models;

    public class QuizCategoriesSeeder : ISeeder
    {
        public async Task SeedAsync(ApplicationDbContext dbContext, IServiceProvider serviceProvider)
        {
            if (dbContext.QuizCategories.Any())
            {
                return;
            }

            await dbContext.QuizCategories.AddAsync(new QuizCategory() { Name = "Arts & Literature" });
            await dbContext.QuizCategories.AddAsync(new QuizCategory() { Name = "Film & TV" });
            await dbContext.QuizCategories.AddAsync(new QuizCategory() { Name = "Food & Drink" });
            await dbContext.QuizCategories.AddAsync(new QuizCategory() { Name = "General Knowledge" });
            await dbContext.QuizCategories.AddAsync(new QuizCategory() { Name = "Geography" });
            await dbContext.QuizCategories.AddAsync(new QuizCategory() { Name = "History" });
            await dbContext.QuizCategories.AddAsync(new QuizCategory() { Name = "Music" });
            await dbContext.QuizCategories.AddAsync(new QuizCategory() { Name = "Science" });
            await dbContext.QuizCategories.AddAsync(new QuizCategory() { Name = "Society & Culture" });
            await dbContext.QuizCategories.AddAsync(new QuizCategory() { Name = "Sport & Leisure" });
        }
    }
}
