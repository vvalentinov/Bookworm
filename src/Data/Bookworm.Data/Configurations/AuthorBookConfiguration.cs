namespace Bookworm.Data.Configurations
{
    using Bookworm.Data.Models;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    public class AuthorBookConfiguration : IEntityTypeConfiguration<AuthorBook>
    {
        public void Configure(EntityTypeBuilder<AuthorBook> authorBook)
        {
            authorBook.HasKey(x => new { x.AuthorId, x.BookId });

            authorBook.HasQueryFilter(x => !x.Book.IsDeleted);
        }
    }
}
