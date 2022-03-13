namespace Bookworm.Web.ViewModels.Books
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    using Bookworm.Web.ViewModels.Categories;
    using Microsoft.AspNetCore.Http;

    using static Bookworm.Common.DataConstants;
    using static Bookworm.Common.ErrorMessages;

    public class UploadBookFormModel
    {
        [Required(ErrorMessage = BookTitleRequired)]
        [StringLength(BookTitleMaxLength, MinimumLength = BookTitleMinLength, ErrorMessage = BookTitleLenght)]
        public string Title { get; set; }

        [Required(ErrorMessage = BookDescriptionRequired)]
        [StringLength(BookDescriptionMaxLength, MinimumLength = BookDescriptionMinLength, ErrorMessage = BookDescriptionLength)]
        public string Description { get; set; }

        [Required(ErrorMessage = BookLanguageRequired)]
        public string Language { get; set; }

        [StringLength(BookPublisherMax, MinimumLength = BookPublisherMin, ErrorMessage = BookPublisherLength)]
        public string Publisher { get; set; }

        [Range(BookPagesCountMin, BookPagesCountMax, ErrorMessage = BookPagesCountRange)]
        public int PagesCount { get; set; }

        public int PublishedYear { get; set; }

        [Required(ErrorMessage = BookFileRequired)]
        public IFormFile BookFile { get; set; }

        [Required(ErrorMessage = BookImageFileRequired)]
        public IFormFile ImageFile { get; set; }

        public int CategoryId { get; set; }

        public IEnumerable<string> AuthorsNames { get; set; }

        public IEnumerable<CategoryViewModel> Categories { get; set; }
    }
}
