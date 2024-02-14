namespace Bookworm.Web.ViewModels.Authors
{
    using System.ComponentModel.DataAnnotations;

    using Bookworm.Data.Models;
    using Bookworm.Services.Mapping;

    using static Bookworm.Common.Authors.AuthorsDataConstants;
    using static Bookworm.Common.Authors.AuthorsErrorMessagesConstants;

    public class UploadAuthorViewModel : IMapFrom<Author>
    {
        public int Id { get; set; }

        [Required(ErrorMessage = RequiredAuthorNameError)]
        [StringLength(AuthorNameMaxLength, MinimumLength = AuthorNameMinLength, ErrorMessage = AuthorNameLengthError)]
        public string Name { get; set; }
    }
}
