namespace Bookworm.Web.ViewModels.Ratings
{
    using System.ComponentModel.DataAnnotations;

    using static Bookworm.Common.Constants.ErrorMessagesConstants;

    public class RatingInputModel
    {
        public int BookId { get; set; }

        [Range(1, 5, ErrorMessage = FieldRangeError)]
        public byte Value { get; set; }
    }
}
