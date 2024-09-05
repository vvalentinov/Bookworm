namespace Bookworm.Data.Configurations
{
    using Bookworm.Data.Models;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    public class FavoriteBookConfiguration : IEntityTypeConfiguration<FavoriteBook>
    {
        public void Configure(EntityTypeBuilder<FavoriteBook> favBook)
        {
            favBook.HasKey(x => new { x.BookId, x.UserId });

            favBook.HasQueryFilter(x => !x.Book.IsDeleted);
        }
    }
}
