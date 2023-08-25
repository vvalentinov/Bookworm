namespace Bookworm.Web.ViewModels.Books
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    using Bookworm.Web.Infrastructure;
    using Bookworm.Web.ViewModels.Authors;
    using Bookworm.Web.ViewModels.Categories;
    using Bookworm.Web.ViewModels.Languages;

    using static Bookworm.Common.Books.BooksDataConstants;
    using static Bookworm.Common.Books.BooksErrorMessagesConstants;

    public class EditBookFormModel
    {
        public string Id { get; set; }

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

        public int CategoryId { get; set; }

        public int LanguageId { get; set; }

        public IEnumerable<AuthorViewModel> AuthorsNames { get; set; }

        public IEnumerable<LanguageViewModel> Languages { get; set; }

        public IEnumerable<CategoryViewModel> Categories { get; set; }
    }
}
