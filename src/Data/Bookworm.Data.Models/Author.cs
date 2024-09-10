namespace Bookworm.Data.Models
{
    using System.ComponentModel.DataAnnotations;

    using Bookworm.Data.Common.Models;

    using static Bookworm.Common.Constants.DataConstants.AuthorDataConstants;
    using static Bookworm.Common.Constants.ErrorMessagesConstants;

    public class Author : BaseModel<int>
    {
        [Required]
        [MaxLength(AuthorNameMaxLength, ErrorMessage = FieldMaxLengthError)]
        public string Name { get; set; }
    }
}
