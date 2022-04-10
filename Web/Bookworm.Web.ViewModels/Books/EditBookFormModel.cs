namespace Bookworm.Web.ViewModels.Books
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    using Bookworm.Web.Infrastructure;
    using Bookworm.Web.ViewModels.Authors;
    using Microsoft.AspNetCore.Mvc.Rendering;

    using static Bookworm.Common.DataConstants;
    using static Bookworm.Common.ErrorMessages;

    public class EditBookFormModel
    {
        public string Id { get; set; }

        [Required(ErrorMessage = BookTitleRequired)]
        [StringLength(BookTitleMaxLength, MinimumLength = BookTitleMinLength, ErrorMessage = BookTitleLenght)]
        public string Title { get; set; }

        [Required(ErrorMessage = BookDescriptionRequired)]
        [StringLength(BookDescriptionMaxLength, MinimumLength = BookDescriptionMinLength, ErrorMessage = BookDescriptionLength)]
        public string Description { get; set; }

        [StringLength(BookPublisherMax, MinimumLength = BookPublisherMin, ErrorMessage = BookPublisherLength)]
        public string Publisher { get; set; }

        [Range(BookPagesCountMin, BookPagesCountMax, ErrorMessage = BookPagesCountRange)]
        public int PagesCount { get; set; }

        [Display(Name = "Year")]
        [PublishedYearValidationAttribute(BookPublishedYearMin, ErrorMessage = "Invalid year value.")]
        public int PublishedYear { get; set; }

        public int CategoryId { get; set; }

        public int LanguageId { get; set; }

        public IEnumerable<AuthorViewModel> AuthorsNames { get; set; }

        public IEnumerable<SelectListItem> Languages { get; set; }

        public IEnumerable<SelectListItem> Categories { get; set; }
    }
}
