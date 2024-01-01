namespace Bookworm.Services.Data.Tests
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Bookworm.Data.Common.Repositories;
    using Bookworm.Data.Models;
    using Bookworm.Data.Models.Enums;
    using Bookworm.Services.Data.Models;
    using Moq;
    using Xunit;

    public class VotesServiceTests
    {
        private readonly List<Vote> votesList;
        private readonly VotesService votesService;

        public VotesServiceTests()
        {
            this.votesList = new List<Vote>();
            Mock<IRepository<Vote>> mockVoteRepo = new Mock<IRepository<Vote>>();
            mockVoteRepo.Setup(x => x.AllAsNoTracking()).Returns(this.votesList.AsQueryable());
            mockVoteRepo.Setup(x => x.All()).Returns(this.votesList.AsQueryable());
            mockVoteRepo.Setup(x => x.AddAsync(It.IsAny<Vote>()))
                .Callback((Vote vote) => this.votesList.Add(vote));

            this.votesService = new VotesService(mockVoteRepo.Object);
        }

        [Fact]
        public async Task UserVoteShouldChangeUponMultipleVoting()
        {
            await this.votesService.VoteAsync(1, "3c96e931-0bec-409e-82c3-3fd088c5b9b5", true);
            await this.votesService.VoteAsync(1, "3c96e931-0bec-409e-82c3-3fd088c5b9b5", false);
            await this.votesService.VoteAsync(1, "3c96e931-0bec-409e-82c3-3fd088c5b9b5", true);

            VoteValue value = this.votesList
                                  .FirstOrDefault(x => x.UserId == "3c96e931-0bec-409e-82c3-3fd088c5b9b5" && x.CommentId == 1)
                                  .Value;
            Assert.Equal(VoteValue.UpVote, value);
        }

        [Fact]
        public async Task CommentUpVotesCountShouldBeCorrect()
        {
            await this.votesService.VoteAsync(1, "3c96e931-0bec-409e-82c3-3fd088c5b9b5", true);
            await this.votesService.VoteAsync(1, "eeacb9b3-2624-4536-b5ce-6c2543a83d99", false);
            await this.votesService.VoteAsync(1, "3c96e931-0bec-409e-82c3-3fd088c5b9b5", true);
            await this.votesService.VoteAsync(1, "8b1ab724-7304-4fe7-b4d6-fd7a376796ac", true);

            await this.votesService.VoteAsync(2, "3be33742-aa74-4198-89d4-7048360e82d1", true);
            await this.votesService.VoteAsync(2, "3be33742-aa74-4198-89d4-7048360e82d1", true);
            await this.votesService.VoteAsync(2, "3be33742-aa74-4198-89d4-7048360e82d1", true);
            await this.votesService.VoteAsync(2, "8b1ab724-7304-4fe7-b4d6-fd7a376796ac", true);
            await this.votesService.VoteAsync(2, "f8b32864-da3c-44a3-8cb3-50efcfe02947", true);
            await this.votesService.VoteAsync(2, "6e62da01-7016-45c4-a967-c5d4d2c179ed", false);

            int firstCommentUpVotesCount = this.votesService.GetUpVotesCount(1);
            int secondCommentUpVotesCount = this.votesService.GetUpVotesCount(2);
            Assert.Equal(2, firstCommentUpVotesCount);
            Assert.Equal(3, secondCommentUpVotesCount);
        }

        [Fact]
        public async Task CommentDownVotesCountShouldBeCorrect()
        {
            await this.votesService.VoteAsync(1, "3c96e931-0bec-409e-82c3-3fd088c5b9b5", true);
            await this.votesService.VoteAsync(1, "3c96e931-0bec-409e-82c3-3fd088c5b9b5", false);
            await this.votesService.VoteAsync(1, "96434435-9577-4325-9d0c-d243d76a82c5", false);

            await this.votesService.VoteAsync(2, "3be33742-aa74-4198-89d4-7048360e82d1", true);
            await this.votesService.VoteAsync(2, "3be33742-aa74-4198-89d4-7048360e82d1", false);
            await this.votesService.VoteAsync(2, "700ada34-2112-4387-9d53-2e0a3ae77448", false);
            await this.votesService.VoteAsync(2, "4a71b8a5-478c-46aa-8a2d-2cbdfb004cac", false);
            await this.votesService.VoteAsync(2, "8b1ab724-7304-4fe7-b4d6-fd7a376796ac", true);

            int firstCommentDownVotesCount = this.votesService.GetDownVotesCount(1);
            int secondCommentDownVotesCount = this.votesService.GetDownVotesCount(2);
            Assert.Equal(2, firstCommentDownVotesCount);
            Assert.Equal(3, secondCommentDownVotesCount);
        }
    }
}
