namespace Bookworm.Web.ViewModels.Books
{
    using System.Collections.Generic;

    public class SearchBookInputModel
    {
        public int Page { get; set; }

        public string Input { get; set; }

        public string UserId { get; set; }

        public int CategoryId { get; set; }

        public string Category { get; set; }

        public bool IsForUserBooks { get; set; }

        public List<int> LanguagesIds { get; set; }
    }
}
