namespace Bookworm.Web.ViewModels.Books
{
    public class SearchBookInputModel
    {
        public string Input { get; set; }

        public int Page { get; set; }

        public int CategoryId { get; set; }

        public string Category { get; set; }

        public string UserId { get; set; }

        public bool IsForUserBooks { get; set; }
    }
}
