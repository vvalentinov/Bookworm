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
            this.Votes = new HashSet<Vote>();
            this.Books = new HashSet<Book>();
            this.FavoriteBooks = new HashSet<FavoriteBook>();
            this.Comments = new HashSet<Comment>();
        }

        public string ProfilePictureUrl { get; set; }

        // Audit info
        public DateTime CreatedOn { get; set; }

        public DateTime? ModifiedOn { get; set; }

        // Deletable entity
        public bool IsDeleted { get; set; }

        public DateTime? DeletedOn { get; set; }

        public virtual ICollection<IdentityUserRole<string>> Roles { get; set; }

        public virtual ICollection<IdentityUserClaim<string>> Claims { get; set; }

        public virtual ICollection<IdentityUserLogin<string>> Logins { get; set; }

        public virtual ICollection<Vote> Votes { get; set; }

        public virtual ICollection<Book> Books { get; set; }

        public virtual ICollection<FavoriteBook> FavoriteBooks { get; set; }

        public virtual ICollection<Comment> Comments { get; set; }
    }
}
