namespace Bookworm.Web.ViewModels.Languages
{
    using Bookworm.Data.Models;
    using Bookworm.Services.Mapping;

    public class LanguageViewModel : IMapFrom<Language>
    {
        public int Id { get; set; }

        public string Name { get; set; }
    }
}
