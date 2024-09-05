namespace Bookworm.Web.ViewModels.Authors
{
    using System.ComponentModel.DataAnnotations;

    using static Bookworm.Common.Constants.DataConstants.AuthorDataConstants;
    using static Bookworm.Common.Constants.ErrorMessagesConstants;

    public class UploadAuthorViewModel
    {
        [Required(ErrorMessage = FieldRequiredError)]
        [StringLength(
            AuthorNameMaxLength,
            MinimumLength = AuthorNameMinLength,
            ErrorMessage = FieldStringLengthError)]
        public string Name { get; set; }
    }
}
