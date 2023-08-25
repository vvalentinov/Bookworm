namespace Bookworm.Web.ViewModels.Books
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    using Bookworm.Data.Models;
    using Bookworm.Services.Mapping;
    using Bookworm.Web.Infrastructure;
    using Bookworm.Web.ViewModels.Authors;
    using Bookworm.Web.ViewModels.Categories;
    using Bookworm.Web.ViewModels.Languages;
    using Microsoft.AspNetCore.Http;

    using static Bookworm.Common.Books.BooksDataConstants;
    using static Bookworm.Common.Books.BooksErrorMessagesConstants;

    public class UploadBookFormModel : IMapFrom<Book>
    {
        [Required(ErrorMessage = BookTitleRequiredError)]
        [StringLength(BookTitleMaxLength, MinimumLength = BookTitleMinLength, ErrorMessage = BookTitleLengthError)]
        public string Title { get; set; }

        [Required(ErrorMessage = BookDescriptionRequiredError)]
        [StringLength(BookDescriptionMaxLength, MinimumLength = BookDescriptionMinLength, ErrorMessage = BookDescriptionLengthError)]
        public string Description { get; set; }

        [StringLength(BookPublisherMaxLength, MinimumLength = BookPublisherMinLength, ErrorMessage = BookPublisherLengthError)]
        public string Publisher { get; set; }

        [Range(BookPagesCountMin, BookPagesCountMax, ErrorMessage = BookPagesCountRangeError)]
        public int PagesCount { get; set; }

        [Display(Name = "Year")]
        [PublishedYearValidationAttribute(BookPublishedYearMin, ErrorMessage = BookPublishedYearInvalidError)]
        public int PublishedYear { get; set; }

        [Required(ErrorMessage = BookFileRequiredError)]
        [BookFileAllowedExtensionAttribute(BookFileAllowedExtension)]
        public IFormFile BookFile { get; set; }

        [Required(ErrorMessage = BookImageFileRequiredError)]
        [ImageFileAllowedExtensionsAttribute(new string[] { ".jpg", ".jpeg", ".png" })]
        public IFormFile ImageFile { get; set; }

        public int CategoryId { get; set; }

        public int LanguageId { get; set; }

        [NotEmptyCollection(ErrorMessage = "You must add at least one author!")]
        public IEnumerable<UploadAuthorViewModel> Authors { get; set; }

        public IEnumerable<LanguageViewModel> Languages { get; set; }

        public IEnumerable<CategoryViewModel> Categories { get; set; }
    }
}
