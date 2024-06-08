namespace Bookworm.Services.Data.Tests
{
    using System;
    using System.Threading.Tasks;

    using Bookworm.Data;
    using Bookworm.Data.Models;
    using Bookworm.Data.Repositories;
    using Bookworm.Services.Data.Models;
    using Bookworm.Services.Data.Tests.Shared;
    using Xunit;

    using static Bookworm.Common.Constants.ErrorMessagesConstants.CommentErrorMessagesConstants;

    public class VotesServiceTests : IClassFixture<DbContextFixture>
    {
        private readonly ApplicationDbContext dbContext;

        public VotesServiceTests(DbContextFixture dbContextFixture)
        {
            this.dbContext = dbContextFixture.DbContext;
        }

        [Fact]
        public async Task VoteShouldWorkCorrectlyWhenUserVotesForFirstTime()
        {
            var service = this.GetVotesService();
            var userId = "0fc3ea28-3165-440e-947e-670c90562320";

            var netWorth = await service.VoteAsync(2, userId, true);

            Assert.Equal(0, netWorth);
        }

        [Fact]
        public async Task VoteShouldWorkCorrectlyWhenUserChangesVote()
        {
            var service = this.GetVotesService();
            var userId = "a84ea5dc-a89e-442f-8e53-c874675bb114";

            var netWorth = await service.VoteAsync(1, userId, false);

            Assert.Equal(0, netWorth);
        }

        [Fact]
        public async Task VoteShouldThrowExceptionIfCommentIdIsInvalid()
        {
            var service = this.GetVotesService();

            var exception = await Assert.ThrowsAsync<InvalidOperationException>(async ()
                => await service.VoteAsync(10, string.Empty, false));

            Assert.Equal(CommentWrongIdError, exception.Message);
        }

        [Fact]
        public async Task VoteShouldThrowExceptionIfCurrUserIsCommentCreator()
        {
            var service = this.GetVotesService();

            var exception = await Assert.ThrowsAsync<InvalidOperationException>(async ()
                => await service.VoteAsync(2, "a84ea5dc-a89e-442f-8e53-c874675bb114", false));

            Assert.Equal(CommentVoteError, exception.Message);
        }

        private VotesService GetVotesService() => new(new EfDeletableEntityRepository<Comment>(this.dbContext));
    }
}
