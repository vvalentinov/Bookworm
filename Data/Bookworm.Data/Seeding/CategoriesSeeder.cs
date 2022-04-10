namespace Bookworm.Data.Seeding
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Bookworm.Data.Models;
    using Microsoft.Extensions.Configuration;

    public class CategoriesSeeder : ISeeder
    {
        private readonly IConfiguration configuration;

        private readonly List<Category> categories;

        public CategoriesSeeder(IConfiguration configuration)
        {
            this.configuration = configuration;

            this.categories = new List<Category>()
            {
                new Category() { Name = "Arts & Music", ImageUrl = this.configuration.GetValue<string>("CategoriesUrls:Arts&Music") },
                new Category() { Name = "Biographies & Autobiographies", ImageUrl = this.configuration.GetValue<string>("CategoriesUrls:Biographies&Autobiographies") },
                new Category() { Name = "Business & Economics", ImageUrl = this.configuration.GetValue<string>("CategoriesUrls:Business&Economics") },
                new Category() { Name = "Comics & Graphic Novels", ImageUrl = this.configuration.GetValue<string>("CategoriesUrls:Comics&GraphicNovels") },
                new Category() { Name = "Cooking", ImageUrl = this.configuration.GetValue<string>("CategoriesUrls:Cooking") },
                new Category() { Name = "Thrillers & Crimes & Mysteries", ImageUrl = this.configuration.GetValue<string>("CategoriesUrls:Crime&Mystery&Thriller") },
                new Category() { Name = "Health & Fitness", ImageUrl = this.configuration.GetValue<string>("CategoriesUrls:Health&Fitness") },
                new Category() { Name = "History", ImageUrl = this.configuration.GetValue<string>("CategoriesUrls:History") },
                new Category() { Name = "Hobbies & Crafts", ImageUrl = this.configuration.GetValue<string>("CategoriesUrls:Hobbies&Crafts") },
                new Category() { Name = "Horror", ImageUrl = this.configuration.GetValue<string>("CategoriesUrls:Horror") },
                new Category() { Name = "Kid's Book's", ImageUrl = this.configuration.GetValue<string>("CategoriesUrls:KidsBooks") },
                new Category() { Name = "Religion", ImageUrl = this.configuration.GetValue<string>("CategoriesUrls:Religion") },
                new Category() { Name = "Romance", ImageUrl = this.configuration.GetValue<string>("CategoriesUrls:Romance") },
                new Category() { Name = "Sci-Fi & Fantasy", ImageUrl = this.configuration.GetValue<string>("CategoriesUrls:Sci Fi&Fantasy") },
                new Category() { Name = "Science & Technology", ImageUrl = this.configuration.GetValue<string>("CategoriesUrls:Science&Technology") },
                new Category() { Name = "Self-help", ImageUrl = this.configuration.GetValue<string>("CategoriesUrls:Self-Help") },
                new Category() { Name = "Sports", ImageUrl = this.configuration.GetValue<string>("CategoriesUrls:Sports") },
                new Category() { Name = "Travel", ImageUrl = this.configuration.GetValue<string>("CategoriesUrls:Travel") },
            };
        }

        public async Task SeedAsync(ApplicationDbContext dbContext, IServiceProvider serviceProvider)
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
