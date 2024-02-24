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

    using static Bookworm.Common.Books.BooksDataConstants;
    using static Bookworm.Common.Books.BooksErrorMessagesConstants;
    using static Bookworm.Common.Publishers.PublishersDataConstants;
    using static Bookworm.Common.Publishers.PublishersErrorMessagesConstants;

    public class EditBookViewModel : IMapFrom<Book>, IMapTo<BookDto>
    {
        public int Id { get; set; }

        [Required(ErrorMessage = BookTitleRequiredError)]
        [StringLength(BookTitleMaxLength, MinimumLength = BookTitleMinLength, ErrorMessage = BookTitleLengthError)]
        public string Title { get; set; }

        [Required(ErrorMessage = BookDescriptionRequiredError)]
        [StringLength(BookDescriptionMaxLength, MinimumLength = BookDescriptionMinLength, ErrorMessage = BookDescriptionLengthError)]
        public string Description { get; set; }

        [Display(Name = "Publisher(optional)")]
        [StringLength(PublisherNameMaxLength, MinimumLength = PublisherNameMinLength, ErrorMessage = PublisherNameLengthError)]
        public string Publisher { get; set; }

        [Display(Name = "Number of pages")]
        [Range(BookPagesCountMin, BookPagesCountMax, ErrorMessage = BookPagesCountRangeError)]
        public int PagesCount { get; set; }

        [Display(Name = "Year")]
        [PublishedYearValidationAttribute(BookPublishedYearMin, ErrorMessage = BookPublishedYearInvalidError)]
        public int PublishedYear { get; set; }

        [Display(Name = "Book PDF file")]
        [BookFileAllowedExtensionAttribute(BookFileAllowedExtension)]
        public IFormFile BookFile { get; set; }

        [Display(Name = "Image File")]
        [ImageFileAllowedExtensionsAttribute([".jpg", ".jpeg", ".png"])]
        public IFormFile ImageFile { get; set; }

        [Display(Name = "Select Book Category")]
        [Required(ErrorMessage = "You must select category!")]
        public int CategoryId { get; set; }

        [Display(Name = "Select Book Language")]
        [Required(ErrorMessage = "You must select language!")]
        public int LanguageId { get; set; }

        [NotEmptyCollection(nameof(Authors))]
        public IList<UploadAuthorViewModel> Authors { get; set; }
    }
}
