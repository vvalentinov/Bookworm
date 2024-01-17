namespace Bookworm.Web.ViewModels.Books
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    using Bookworm.Web.Infrastructure.Attributes;
    using Bookworm.Web.ViewModels.Authors;
    using Bookworm.Web.ViewModels.Categories;
    using Bookworm.Web.ViewModels.Languages;
    using Ganss.Xss;
    using Microsoft.AspNetCore.Http;

    using static Bookworm.Common.Books.BooksDataConstants;
    using static Bookworm.Common.Books.BooksErrorMessagesConstants;

    public class EditBookViewModel
    {
        public string Id { get; set; }

        [Required(ErrorMessage = BookTitleRequiredError)]
        [StringLength(BookTitleMaxLength, MinimumLength = BookTitleMinLength, ErrorMessage = BookTitleLengthError)]
        public string Title { get; set; }

        [Required(ErrorMessage = BookDescriptionRequiredError)]
        [StringLength(BookDescriptionMaxLength, MinimumLength = BookDescriptionMinLength, ErrorMessage = BookDescriptionLengthError)]
        public string Description { get; set; }

        public string SanitizedDescription => new HtmlSanitizer().Sanitize(this.Description);

        [StringLength(BookPublisherMaxLength, MinimumLength = BookPublisherMinLength, ErrorMessage = BookPublisherLengthError)]
        public string Publisher { get; set; }

        [Range(BookPagesCountMin, BookPagesCountMax, ErrorMessage = BookPagesCountRangeError)]
        public int PagesCount { get; set; }

        [Display(Name = "Year")]
        [PublishedYearValidationAttribute(BookPublishedYearMin, ErrorMessage = BookPublishedYearInvalidError)]
        public int PublishedYear { get; set; }

        [Display(Name = "Image File")]
        [ImageFileAllowedExtensionsAttribute([".jpg", ".jpeg", ".png"])]
        public IFormFile ImageFile { get; set; }

        [Display(Name = "Book PDF file")]
        [BookFileAllowedExtensionAttribute(BookFileAllowedExtension)]
        public IFormFile BookFile { get; set; }

        public string ImageUrl { get; set; }

        public int CategoryId { get; set; }

        public int LanguageId { get; set; }

        [NotEmptyCollection("authors")]
        public IList<UploadAuthorViewModel> Authors { get; set; }

        public IEnumerable<LanguageViewModel> Languages { get; set; }

        public IEnumerable<CategoryViewModel> Categories { get; set; }
    }
}
