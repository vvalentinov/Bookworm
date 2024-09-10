namespace Bookworm.Data.Models
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    using Bookworm.Data.Common.Models;
    using Microsoft.AspNetCore.Http;

    using static Bookworm.Common.Constants.DataConstants.BookDataConstants;
    using static Bookworm.Common.Constants.ErrorMessagesConstants;

    public class Book :
        BaseDeletableModel<int>,
        IApprovableEntity
    {
        [Required]
        [MaxLength(BookTitleMaxLength, ErrorMessage = FieldMaxLengthError)]
        public string Title { get; set; }

        [Required]
        [MaxLength(BookDescriptionMaxLength, ErrorMessage = FieldMaxLengthError)]
        public string Description { get; set; }

        [Required]
        public int Year { get; set; }

        [Required]
        public int PagesCount { get; set; }

        [Required]
        public int DownloadsCount { get; set; }

        [Required]
        public bool IsApproved { get; set; }

        [NotMapped]
        public IFormFile ImageFile { get; set; }

        [Required]
        public string ImageUrl { get; set; }

        [NotMapped]
        public IFormFile BookFile { get; set; }

        [Required]
        public string FileUrl { get; set; }

        [Required]
        [ForeignKey(nameof(Category))]
        public int CategoryId { get; set; }

        public Category Category { get; set; }

        [Required]
        [ForeignKey(nameof(Language))]
        public int LanguageId { get; set; }

        public Language Language { get; set; }

        [ForeignKey(nameof(Publisher))]
        public int? PublisherId { get; set; }

        public Publisher Publisher { get; set; }

        [Required]
        [ForeignKey(nameof(User))]
        public string UserId { get; set; }

        public ApplicationUser User { get; set; }

        public ICollection<Rating> Ratings { get; set; }
            = new HashSet<Rating>();

        public ICollection<Comment> Comments { get; set; }
            = new HashSet<Comment>();

        public ICollection<AuthorBook> AuthorsBooks { get; set; }
             = new HashSet<AuthorBook>();
    }
}
