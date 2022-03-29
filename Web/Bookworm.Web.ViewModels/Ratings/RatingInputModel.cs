namespace Bookworm.Web.ViewModels.Votes
{
    using System.ComponentModel.DataAnnotations;

    public class RatingInputModel
    {
        public string BookId { get; set; }

        [Range(1, 5)]
        public byte Value { get; set; }
    }
}
