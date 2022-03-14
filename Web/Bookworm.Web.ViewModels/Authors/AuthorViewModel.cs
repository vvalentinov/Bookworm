namespace Bookworm.Web.ViewModels.Authors
{
    using Bookworm.Data.Models;
    using Bookworm.Services.Mapping;

    public class AuthorViewModel : IMapFrom<Author>
    {
        public string Name { get; set; }
    }
}
