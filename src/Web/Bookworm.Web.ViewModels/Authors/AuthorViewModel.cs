namespace Bookworm.Web.ViewModels.Authors
{
    using Bookworm.Data.Models;

    public class AuthorViewModel
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public static AuthorViewModel MapFromAuthor(Author author)
        {
            return new AuthorViewModel
            {
                Id = author.Id,
                Name = author.Name,
            };
        }
    }
}
