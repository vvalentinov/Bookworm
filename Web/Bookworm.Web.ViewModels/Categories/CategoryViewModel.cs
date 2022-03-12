namespace Bookworm.Web.ViewModels.Categories
{
    using Bookworm.Data.Models;
    using Bookworm.Services.Mapping;

    public class CategoryViewModel : IMapFrom<Category>
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string ImageUrl { get; set; }
    }
}
