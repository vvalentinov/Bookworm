namespace Bookworm.Data.Configurations
{
    using Bookworm.Data.Models;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    public class QuoteLikeConfiguration : IEntityTypeConfiguration<QuoteLike>
    {
        public void Configure(EntityTypeBuilder<QuoteLike> quoteLike)
        {
            quoteLike.HasKey(x => new { x.QuoteId, x.UserId });

            quoteLike.HasQueryFilter(x => !x.Quote.IsDeleted);
        }
    }
}
