namespace Bookworm.Data.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    using Bookworm.Data.Common.Models;
    using Microsoft.AspNetCore.Http;

    using static Bookworm.Common.DataConstants;

    public class Book : BaseDeletableModel<string>
    {
        public Book()
        {
            this.Id = Guid.NewGuid().ToString();
            this.AuthorsBooks = new HashSet<AuthorBook>();
        }

        [Required]
        [MaxLength(BookTitleMaxLength)]
        public string Title { get; set; }

        [Required]
        [MaxLength(BookDescriptionMaxLength)]
        public string Description { get; set; }

        public int Year { get; set; }

        public int PagesCount { get; set; }

        public int DownloadsCount { get; set; }

        [NotMapped]
        public IFormFile ImageFile { get; set; }

        [Required]
        public string ImageUrl { get; set; }

        [NotMapped]
        public IFormFile BookFile { get; set; }

        [Required]
        public string FileUrl { get; set; }

        [ForeignKey(nameof(Category))]
        public int CategoryId { get; set; }

        public virtual Category Category { get; set; }

        [ForeignKey(nameof(Language))]
        public int LanguageId { get; set; }

        public virtual Language Language { get; set; }

        [ForeignKey(nameof(Publisher))]
        public int PublisherId { get; set; }

        public virtual Publisher Publisher { get; set; }

        [ForeignKey(nameof(User))]
        public string UserId { get; set; }

        public virtual ApplicationUser User { get; set; }

        public virtual ICollection<AuthorBook> AuthorsBooks { get; set; }
    }
}
