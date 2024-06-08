namespace Bookworm.Data.Seeding
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Bookworm.Data.Models;

    public class CategoriesSeeder : ISeeder
    {
        private readonly List<Category> categories;

        public CategoriesSeeder()
        {
            this.categories =
            [
                new () { Name = "Arts & Music" },
                new () { Name = "Biographies & Autobiographies" },
                new () { Name = "Business & Economics" },
                new () { Name = "Comics & Graphic Novels" },
                new () { Name = "Cooking" },
                new () { Name = "Thrillers & Crimes & Mysteries" },
                new () { Name = "Health & Fitness" },
                new () { Name = "History" },
                new () { Name = "Hobbies & Crafts" },
                new () { Name = "Horror" },
                new () { Name = "Kid's Book's" },
                new () { Name = "Religion" },
                new () { Name = "Romance" },
                new () { Name = "Sci-Fi & Fantasy" },
                new () { Name = "Science & Technology" },
                new () { Name = "Self-help" },
                new () { Name = "Sports" },
                new () { Name = "Travel" },
            ];
        }

        public async Task SeedAsync(
            ApplicationDbContext dbContext,
            IServiceProvider serviceProvider)
        {
            if (dbContext.Categories.Any())
            {
                return;
            }

            await dbContext.Categories.AddRangeAsync(this.categories);
            await dbContext.SaveChangesAsync();
        }
    }
}
