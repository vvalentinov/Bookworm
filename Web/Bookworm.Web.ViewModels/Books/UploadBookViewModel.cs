namespace Bookworm.Web.ViewModels.Books
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    using Bookworm.Data.Models;
    using Bookworm.Services.Mapping;
    using Bookworm.Web.Infrastructure.Attributes;
    using Bookworm.Web.ViewModels.Authors;
    using Bookworm.Web.ViewModels.Categories;
    using Bookworm.Web.ViewModels.Languages;
    using Microsoft.AspNetCore.Http;

    using static Bookworm.Common.Books.BooksDataConstants;
    using static Bookworm.Common.Books.BooksErrorMessagesConstants;

    public class UploadBookViewModel : IMapFrom<Book>
    {
        [Required(ErrorMessage = BookTitleRequiredError)]
        [StringLength(BookTitleMaxLength, MinimumLength = BookTitleMinLength, ErrorMessage = BookTitleLengthError)]
        public string Title { get; set; }

        [Display(Name = nameof(Description))]
        [Required(ErrorMessage = BookDescriptionRequiredError)]
        [StringLength(BookDescriptionMaxLength, MinimumLength = BookDescriptionMinLength, ErrorMessage = BookDescriptionLengthError)]
        public string Description { get; set; }

        [Display(Name = "Publisher(optional)")]
        [StringLength(BookPublisherMaxLength, MinimumLength = BookPublisherMinLength, ErrorMessage = BookPublisherLengthError)]
        public string Publisher { get; set; }

        [Display(Name = "Number of pages")]
        [Range(BookPagesCountMin, BookPagesCountMax, ErrorMessage = BookPagesCountRangeError)]
        public int PagesCount { get; set; }

        [Display(Name = "Year")]
        [PublishedYearValidationAttribute(BookPublishedYearMin, ErrorMessage = BookPublishedYearInvalidError)]
        public int PublishedYear { get; set; }

        [Display(Name = "Book PDF file")]
        [Required(ErrorMessage = BookFileRequiredError)]
        [BookFileAllowedExtensionAttribute(BookFileAllowedExtension)]
        public IFormFile BookFile { get; set; }

        [Display(Name = "Image File")]
        [Required(ErrorMessage = BookImageFileRequiredError)]
        [ImageFileAllowedExtensionsAttribute(new string[] { ".jpg", ".jpeg", ".png" })]
        public IFormFile ImageFile { get; set; }

        [Display(Name = "Select Book Category")]
        [Required(ErrorMessage = "You must select category!")]
        public int CategoryId { get; set; }

        [Display(Name = "Select Book Language")]
        [Required(ErrorMessage = "You must select language!")]
        public int LanguageId { get; set; }

        [NotEmptyCollection("authors")]
        public IEnumerable<UploadAuthorViewModel> Authors { get; set; }
    }
}
