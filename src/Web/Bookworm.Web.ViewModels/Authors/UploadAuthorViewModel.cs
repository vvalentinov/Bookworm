namespace Bookworm.Web.ViewModels.Authors
{
    using System.ComponentModel.DataAnnotations;

    using Bookworm.Data.Models;
    using Bookworm.Services.Mapping;

    using static Bookworm.Common.Constants.DataConstants.AuthorDataConstants;
    using static Bookworm.Common.Constants.ErrorMessagesConstants;

    public class UploadAuthorViewModel : IMapFrom<Author>
    {
        [Required(ErrorMessage = FieldRequiredError)]
        [StringLength(
            AuthorNameMaxLength,
            MinimumLength = AuthorNameMinLength,
            ErrorMessage = FieldStringLengthError)]
        public string Name { get; set; }
    }
}
