namespace Bookworm.Web.ViewModels.Books
{
    using Bookworm.Web.Infrastructure.Attributes;

    public class RandomBookInputModel
    {
        public int? CategoryId { get; set; }

        [RandomBooksCountValidation]
        public int CountBooks { get; set; }
    }
}
