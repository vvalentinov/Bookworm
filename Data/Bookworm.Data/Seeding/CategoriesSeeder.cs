namespace Bookworm.Data.Seeding
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    using Bookworm.Data.Models;
    using Microsoft.Extensions.Configuration;

    public class CategoriesSeeder : ISeeder
    {
        private readonly IConfiguration configuration;

        public CategoriesSeeder(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public async Task SeedAsync(ApplicationDbContext dbContext, IServiceProvider serviceProvider)
        {
            if (dbContext.Categories.Any())
            {
                return;
            }

            await dbContext.Categories.AddAsync(new Category { Name = "Arts & Music", ImageUrl = this.configuration.GetValue<string>("CategoriesUrls:Arts&Music") });
            await dbContext.Categories.AddAsync(new Category { Name = "Biographies & Autobiographies", ImageUrl = this.configuration.GetValue<string>("CategoriesUrls:Biographies&Autobiographies") });
            await dbContext.Categories.AddAsync(new Category { Name = "Business & Economics", ImageUrl = this.configuration.GetValue<string>("CategoriesUrls:Business&Economics") });
            await dbContext.Categories.AddAsync(new Category { Name = "Comics & Graphic Novels", ImageUrl = this.configuration.GetValue<string>("CategoriesUrls:Comics&GraphicNovels") });
            await dbContext.Categories.AddAsync(new Category { Name = "Cooking", ImageUrl = this.configuration.GetValue<string>("CategoriesUrls:Cooking") });
            await dbContext.Categories.AddAsync(new Category { Name = "Thrillers & Crimes & Mysteries", ImageUrl = this.configuration.GetValue<string>("CategoriesUrls:Crime&Mystery&Thriller") });
            await dbContext.Categories.AddAsync(new Category { Name = "Health & Fitness", ImageUrl = this.configuration.GetValue<string>("CategoriesUrls:Health&Fitness") });
            await dbContext.Categories.AddAsync(new Category { Name = "History", ImageUrl = this.configuration.GetValue<string>("CategoriesUrls:History") });
            await dbContext.Categories.AddAsync(new Category { Name = "Hobbies & Crafts", ImageUrl = this.configuration.GetValue<string>("CategoriesUrls:Hobbies&Crafts") });
            await dbContext.Categories.AddAsync(new Category { Name = "Horror", ImageUrl = this.configuration.GetValue<string>("CategoriesUrls:Horror") });
            await dbContext.Categories.AddAsync(new Category { Name = "Kid's Book's", ImageUrl = this.configuration.GetValue<string>("CategoriesUrls:KidsBooks") });
            await dbContext.Categories.AddAsync(new Category { Name = "Religion", ImageUrl = this.configuration.GetValue<string>("CategoriesUrls:Religion") });
            await dbContext.Categories.AddAsync(new Category { Name = "Romance", ImageUrl = this.configuration.GetValue<string>("CategoriesUrls:Romance") });
            await dbContext.Categories.AddAsync(new Category { Name = "Sci-Fi & Fantasy", ImageUrl = this.configuration.GetValue<string>("CategoriesUrls:Sci Fi&Fantasy") });
            await dbContext.Categories.AddAsync(new Category { Name = "Science & Technology", ImageUrl = this.configuration.GetValue<string>("CategoriesUrls:Science&Technology") });
            await dbContext.Categories.AddAsync(new Category { Name = "Self-help", ImageUrl = this.configuration.GetValue<string>("CategoriesUrls:Self-Help") });
            await dbContext.Categories.AddAsync(new Category { Name = "Sports", ImageUrl = this.configuration.GetValue<string>("CategoriesUrls:Sports") });
            await dbContext.Categories.AddAsync(new Category { Name = "Travel", ImageUrl = this.configuration.GetValue<string>("CategoriesUrls:Travel") });

            await dbContext.SaveChangesAsync();
        }
    }
}
