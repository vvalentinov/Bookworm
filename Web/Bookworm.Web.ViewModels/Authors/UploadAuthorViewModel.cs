namespace Bookworm.Web.ViewModels.Authors
{
    using System.ComponentModel.DataAnnotations;

    using static Bookworm.Common.Authors.AuthorsDataConstants;
    using static Bookworm.Common.Authors.AuthorsErrorMessagesConstants;

    public class UploadAuthorViewModel
    {
        [Required]
        [StringLength(AuthorNameMaxLength, MinimumLength = AuthorNameMinLength, ErrorMessage = InvalidAuthorNameLengthError)]
        public string Name { get; set; }
    }
}
