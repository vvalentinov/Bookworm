namespace Bookworm.Services.Data.Tests
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Bookworm.Data.Common.Repositories;
    using Bookworm.Data.Models;
    using Bookworm.Services.Data.Models;
    using Moq;
    using Xunit;

    public class RatingsServiceTests
    {
        private readonly List<Rating> ratingList;
        private readonly RatingsService ratingsService;

        public RatingsServiceTests()
        {
            this.ratingList = new List<Rating>()
            {
                new Rating()
                {
                    BookId = "f3f8888a-9ed2-492b-9775-fc6eb804e8bf",
                    UserId = "05078ba1-9ac5-4049-b240-3ba97eb5aa35",
                    Value = 2,
                },
                new Rating()
                {
                    BookId = "488b1766-ba73-476d-8dca-18750fc79d38",
                    UserId = "3e129ed3-0eac-41ab-8508-5b8f005b13a2",
                    Value = 1,
                },
            };
            Mock<IRepository<Rating>> mockRatingRepo = new Mock<IRepository<Rating>>();
            mockRatingRepo.Setup(x => x.AllAsNoTracking()).Returns(this.ratingList.AsQueryable());
            mockRatingRepo.Setup(x => x.All()).Returns(this.ratingList.AsQueryable());
            mockRatingRepo.Setup(x => x.AddAsync(It.IsAny<Rating>()))
                .Callback((Rating rating) => this.ratingList.Add(rating));

            this.ratingsService = new RatingsService(mockRatingRepo.Object);
        }

        [Fact]
        public async Task UponRatingMultipleTimesOnlyOneRatingShouldBeRegistered()
        {
            await this.ratingsService.SetVoteAsync("f3f8888a-9ed2-492b-9775-fc6eb804e8bf", "05078ba1-9ac5-4049-b240-3ba97eb5aa35", 5);
            await this.ratingsService.SetVoteAsync("488b1766-ba73-476d-8dca-18750fc79d38", "3e129ed3-0eac-41ab-8508-5b8f005b13a2", 4);

            int firstUserRating = this.ratingList.FirstOrDefault(x => x.UserId == "05078ba1-9ac5-4049-b240-3ba97eb5aa35").Value;
            int secondUserRating = this.ratingList.FirstOrDefault(x => x.UserId == "3e129ed3-0eac-41ab-8508-5b8f005b13a2").Value;

            Assert.Equal(2, this.ratingList.Count);
            Assert.Equal(5, firstUserRating);
            Assert.Equal(4, secondUserRating);
        }

        [Fact]
        public async Task AverageRatingShouldWorkCorrectly()
        {
            await this.ratingsService.SetVoteAsync("f3f8888a-9ed2-492b-9775-fc6eb804e8bf", "e3cb0d91-1a38-4520-8e3e-cc65255f10fe", 5);
            await this.ratingsService.SetVoteAsync("488b1766-ba73-476d-8dca-18750fc79d38", "94252f24-2edb-495e-9e40-a9b969d4fd46", 4);

            var averageRatingFirstBook = this.ratingsService.GetAverageVotes("f3f8888a-9ed2-492b-9775-fc6eb804e8bf");
            var averageRatingSecondBook = this.ratingsService.GetAverageVotes("488b1766-ba73-476d-8dca-18750fc79d38");

            Assert.Equal(3.5, averageRatingFirstBook);
            Assert.Equal(2.5, averageRatingSecondBook);
        }

        [Fact]
        public async Task RatingsCountShouldBeCorrect()
        {
            await this.ratingsService.SetVoteAsync("f3f8888a-9ed2-492b-9775-fc6eb804e8bf", "715f5faf-5996-4cd9-82a4-42a1cdc9d948", 3);

            int ratingsCount = this.ratingsService.GetVotesCount("f3f8888a-9ed2-492b-9775-fc6eb804e8bf");

            Assert.Equal(2, ratingsCount);
        }

        [Fact]
        public async Task UserRatingShouldBeCorrect()
        {
            await this.ratingsService.SetVoteAsync("4417b951-6549-43ae-ae61-b693b7e38955", "715f5faf-5996-4cd9-82a4-42a1cdc9d948", 1);
            await this.ratingsService.SetVoteAsync("4417b951-6549-43ae-ae61-b693b7e38955", "6283178f-8831-4824-8cdd-27de288882fe", 3);
            await this.ratingsService.SetVoteAsync("4417b951-6549-43ae-ae61-b693b7e38955", "d899ee9a-a0ae-4b49-8d2d-253bbb3f35b9", 5);
            await this.ratingsService.SetVoteAsync("4417b951-6549-43ae-ae61-b693b7e38955", "ba68af0c-6add-488c-8a6e-5867f72f866a", 2);
            await this.ratingsService.SetVoteAsync("4417b951-6549-43ae-ae61-b693b7e38955", "ba68af0c-6add-488c-8a6e-5867f72f866a", 1);

            Assert.Equal(1, this.ratingsService.GetUserVote("4417b951-6549-43ae-ae61-b693b7e38955", "715f5faf-5996-4cd9-82a4-42a1cdc9d948"));
            Assert.Equal(3, this.ratingsService.GetUserVote("4417b951-6549-43ae-ae61-b693b7e38955", "6283178f-8831-4824-8cdd-27de288882fe"));
            Assert.Equal(5, this.ratingsService.GetUserVote("4417b951-6549-43ae-ae61-b693b7e38955", "d899ee9a-a0ae-4b49-8d2d-253bbb3f35b9"));
            Assert.Equal(1, this.ratingsService.GetUserVote("4417b951-6549-43ae-ae61-b693b7e38955", "ba68af0c-6add-488c-8a6e-5867f72f866a"));
        }
    }
}
