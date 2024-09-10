namespace Bookworm.Web.ViewModels.Ratings
{
    using System.ComponentModel.DataAnnotations;

    using static Bookworm.Common.Constants.DataConstants.RatingDataConstants;
    using static Bookworm.Common.Constants.ErrorMessagesConstants;

    public class RatingInputModel
    {
        public int BookId { get; set; }

        [Range(RatingValueMin, RatingValueMax, ErrorMessage = FieldRangeError)]
        public byte Value { get; set; }
    }
}
