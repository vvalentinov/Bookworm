namespace Bookworm.Data.Configurations
{
    using Bookworm.Data.Models;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    public class RatingConfiguration : IEntityTypeConfiguration<Rating>
    {
        public void Configure(EntityTypeBuilder<Rating> rating)
        {
            rating.HasQueryFilter(x => !x.Book.IsDeleted);
        }
    }
}
