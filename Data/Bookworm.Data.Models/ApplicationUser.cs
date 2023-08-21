// ReSharper disable VirtualMemberCallInConstructor
namespace Bookworm.Data.Models
{
    using System;
    using System.Collections.Generic;

    using Bookworm.Data.Common.Models;

    using Microsoft.AspNetCore.Identity;

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

        // Audit info
        public DateTime CreatedOn { get; set; }

        public DateTime? ModifiedOn { get; set; }

        // Deletable entity
        public bool IsDeleted { get; set; }

        public DateTime? DeletedOn { get; set; }

        public virtual ICollection<IdentityUserRole<string>> Roles { get; set; }

        public virtual ICollection<IdentityUserClaim<string>> Claims { get; set; }

        public virtual ICollection<IdentityUserLogin<string>> Logins { get; set; }

        public virtual ICollection<Rating> Ratings { get; set; }

        public virtual ICollection<Vote> Votes { get; set; }

        public virtual ICollection<Book> Books { get; set; }

        public virtual ICollection<FavoriteBook> FavoriteBooks { get; set; }

        public virtual ICollection<Comment> Comments { get; set; }

        public virtual ICollection<Quote> Quotes { get; set; }
    }
}
