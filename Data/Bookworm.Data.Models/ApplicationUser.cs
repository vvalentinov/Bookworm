namespace Bookworm.Data.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    using Bookworm.Data.Common.Models;

    using Microsoft.AspNetCore.Identity;

    using static Bookworm.Common.Constants.DataConstants.ApplicationUser;

    public class ApplicationUser : IdentityUser, IAuditInfo, IDeletableEntity
    {
        public ApplicationUser()
        {
            this.Id = Guid.NewGuid().ToString();
            this.Roles = new HashSet<IdentityUserRole<string>>();
            this.Claims = new HashSet<IdentityUserClaim<string>>();
            this.Logins = new HashSet<IdentityUserLogin<string>>();
            this.Ratings = new HashSet<Rating>();
            this.Votes = new HashSet<Vote>();
            this.Books = new HashSet<Book>();
            this.FavoriteBooks = new HashSet<FavoriteBook>();
            this.Comments = new HashSet<Comment>();
            this.Quotes = new HashSet<Quote>();
        }

        public int Points { get; set; }

        [Range(0, UserMaxDailyBookDownloadsCount)]
        public byte DailyDownloadsCount { get; set; }

        public DateTime CreatedOn { get; set; }

        public DateTime? ModifiedOn { get; set; }

        public bool IsDeleted { get; set; }

        public DateTime? DeletedOn { get; set; }

        public ICollection<IdentityUserRole<string>> Roles { get; set; }

        public ICollection<IdentityUserClaim<string>> Claims { get; set; }

        public ICollection<IdentityUserLogin<string>> Logins { get; set; }

        public ICollection<Rating> Ratings { get; set; }

        public ICollection<Vote> Votes { get; set; }

        public ICollection<Book> Books { get; set; }

        public ICollection<FavoriteBook> FavoriteBooks { get; set; }

        public ICollection<Comment> Comments { get; set; }

        public ICollection<Quote> Quotes { get; set; }
    }
}
