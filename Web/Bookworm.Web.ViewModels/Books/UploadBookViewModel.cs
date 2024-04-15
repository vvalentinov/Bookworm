namespace Bookworm.Web.ViewModels.Books
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    using Bookworm.Data.Models;
    using Bookworm.Services.Mapping;
    using Bookworm.Web.Infrastructure.Attributes;
    using Bookworm.Web.ViewModels.Authors;
    using Bookworm.Web.ViewModels.DTOs;
    using Microsoft.AspNetCore.Http;

    using static Bookworm.Common.Constants.DataConstants.BookDataConstants;
    using static Bookworm.Common.Constants.DataConstants.PublisherDataConstants;
    using static Bookworm.Common.Constants.ErrorMessagesConstants;

    public class UploadBookViewModel : IMapFrom<Book>, IMapTo<BookDto>
    {
        public int Id { get; set; }

        [Required(ErrorMessage = FieldRequiredError)]
        [StringLength(
            BookTitleMaxLength,
            MinimumLength = BookTitleMinLength,
            ErrorMessage = FieldStringLengthError)]
        public string Title { get; set; }

        [Required(ErrorMessage = FieldRequiredError)]
        [StringLength(
            BookDescriptionMaxLength,
            MinimumLength = BookDescriptionMinLength,
            ErrorMessage = FieldStringLengthError)]
        public string Description { get; set; }

        [StringLength(
            PublisherNameMaxLength,
            MinimumLength = PublisherNameMinLength,
            ErrorMessage = FieldStringLengthError)]
        public string Publisher { get; set; }

        [Range(
            BookPagesCountMin,
            BookPagesCountMax,
            ErrorMessage = FieldRangeError)]
        public int PagesCount { get; set; }

        [BookYearValidationAttribute(BookPublishedYearMin)]
        public int Year { get; set; }

        [Display(Name = "PDF file (Max - 15 MB)")]
        [FileAllowedExtensionsAttribute([BookFileAllowedExtension])]
        public IFormFile BookFile { get; set; }

        [Display(Name = "Image File (Max - 5 MB)")]
        [FileAllowedExtensionsAttribute([".jpg", ".jpeg", ".png"])]
        public IFormFile ImageFile { get; set; }

        [Display(Name = nameof(Category))]
        [Required(ErrorMessage = FieldRequiredError)]
        public int CategoryId { get; set; }

        [Display(Name = nameof(Language))]
        [Required(ErrorMessage = FieldRequiredError)]
        public int LanguageId { get; set; }

        [NotEmptyCollection(nameof(Authors))]
        public IList<UploadAuthorViewModel> Authors { get; set; }
    }
}
