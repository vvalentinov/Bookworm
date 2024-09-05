namespace Bookworm.Data.Configurations
{
    using Bookworm.Data.Models;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    public class VoteConfiguration : IEntityTypeConfiguration<Vote>
    {
        public void Configure(EntityTypeBuilder<Vote> vote)
        {
            vote.HasQueryFilter(x => !x.User.IsDeleted);
        }
    }
}
