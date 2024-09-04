namespace Bookworm.Web.ViewModels.Books
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;

    using Bookworm.Data.Models;
    using Bookworm.Web.Infrastructure.ValidationAttributes;
    using Bookworm.Web.ViewModels.Authors;
    using Bookworm.Web.ViewModels.DTOs;
    using Microsoft.AspNetCore.Http;

    using static Bookworm.Common.Constants.DataConstants.BookDataConstants;
    using static Bookworm.Common.Constants.DataConstants.PublisherDataConstants;
    using static Bookworm.Common.Constants.ErrorMessagesConstants;

    public class UploadBookViewModel
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

        [Display(Name = "Pages Count")]
        [Range(
            BookPagesCountMin,
            BookPagesCountMax,
            ErrorMessage = FieldRangeError)]
        public int PagesCount { get; set; }

        [BookYearValidation(BookPublishedYearMin)]
        public int Year { get; set; }

        [Display(Name = "Pdf file (Max - 15 MB)")]
        [FileValidation(BookPdfMaxSize, [BookFileAllowedExtension])]
        public IFormFile BookFile { get; set; }

        [Display(Name = "Image file (Max - 5 MB)")]
        [FileValidation(BookImageMaxSize, [".jpg", ".jpeg", ".png"])]
        public IFormFile ImageFile { get; set; }

        [Display(Name = nameof(Category))]
        [Required(ErrorMessage = FieldRequiredError)]
        public int CategoryId { get; set; }

        [Display(Name = nameof(Language))]
        [Required(ErrorMessage = FieldRequiredError)]
        public int LanguageId { get; set; }

        [AuthorsValidation]
        public IList<UploadAuthorViewModel> Authors { get; set; }

        public static UploadBookViewModel MapFromBook(Book model)
        {
            var authors = model.AuthorsBooks
                .Select(ab => new UploadAuthorViewModel
                {
                    Name = ab.Author.Name,
                })
                .ToList();

            return new UploadBookViewModel
            {
                Id = model.Id,
                Year = model.Year,
                Title = model.Title,
                BookFile = model.BookFile,
                CategoryId = model.CategoryId,
                Description = model.Description,
                ImageFile = model.ImageFile,
                LanguageId = model.LanguageId,
                PagesCount = model.PagesCount,
                Publisher = model.Publisher.Name,
                Authors = authors,
            };
        }

        public BookDto MapToBookDto()
        {
            return new BookDto
            {
                Id = this.Id,
                Authors = this.Authors,
                BookFile = this.BookFile,
                CategoryId = this.CategoryId,
                Description = this.Description,
                ImageFile = this.ImageFile,
                LanguageId = this.LanguageId,
                PagesCount = this.PagesCount,
                Publisher = this.Publisher,
                Title = this.Title,
                Year = this.Year,
            };
        }
    }
}
