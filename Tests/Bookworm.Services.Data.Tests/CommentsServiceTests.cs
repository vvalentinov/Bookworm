//namespace Bookworm.Services.Data.Tests
//{
//    using System.Collections.Generic;
//    using System.Linq;
//    using System.Threading.Tasks;

//    using Bookworm.Data.Common.Repositories;
//    using Bookworm.Data.Models;
//    using Bookworm.Data.Models.Enums;
//    using Bookworm.Services.Data.Models;
//    using Moq;
//    using Xunit;

//    public class CommentsServiceTests
//    {
//        private readonly IList<Comment> comments;
//        private readonly IList<Vote> votes;
//        private readonly CommentsService commentsService;

//        public CommentsServiceTests()
//        {
//            this.votes = new List<Vote>()
//            {
//                new Vote()
//                {
//                    CommentId = 1,
//                    Value = VoteValue.UpVote,
//                    UserId = "93535361-50c4-4c49-9684-6b826deae12c",
//                },
//                new Vote()
//                {
//                    CommentId = 2,
//                    Value = VoteValue.DownVote,
//                    UserId = "ea0dde88-34ce-4d24-b4bd-89ac819c6d31",
//                },
//            };

//            this.comments = new List<Comment>()
//            {
//                new Comment()
//                {
//                    Id = 1,
//                    BookId = "648e17a5-fbc6-4ebe-97f4-4d64ed7074ba",
//                    Content = "First comment content",
//                    UserId = "2ce0609c-7492-47cf-a09b-14e6b329e5d7",
//                    Votes = this.votes.Where(v => v.CommentId == 1).ToList(),
//                },
//                new Comment()
//                {
//                    Id = 2,
//                    BookId = "dba20219-cc5a-4bc3-b5d9-b082ecb6d1f0",
//                    Content = "Second comment content",
//                    UserId = "57437970-e5f8-4832-85a7-f621f70b769e",
//                    Votes = this.votes.Where(v => v.CommentId == 2).ToList(),
//                },
//            };

//            Mock<IRepository<Vote>> mockVoteRepo = new Mock<IRepository<Vote>>();
//            mockVoteRepo.Setup(x => x.All()).Returns(this.votes.AsQueryable());
//            mockVoteRepo.Setup(x => x.AllAsNoTracking()).Returns(this.votes.AsQueryable());
//            mockVoteRepo.Setup(x => x.AddAsync(It.IsAny<Vote>()))
//                .Callback((Vote vote) => this.votes.Add(vote));
//            mockVoteRepo.Setup(x => x.Delete(It.IsAny<Vote>()))
//                .Callback((Vote vote) => this.votes.Remove(vote));

//            Mock<IRepository<Comment>> mockCommentsRepo = new Mock<IRepository<Comment>>();
//            mockCommentsRepo.Setup(x => x.All()).Returns(this.comments.AsQueryable());
//            mockCommentsRepo.Setup(x => x.AllAsNoTracking()).Returns(this.comments.AsQueryable());
//            mockCommentsRepo.Setup(x => x.AddAsync(It.IsAny<Comment>()))
//                .Callback((Comment comment) => this.comments.Add(comment));
//            mockCommentsRepo.Setup(x => x.Delete(It.IsAny<Comment>()))
//                .Callback((Comment comment) => this.comments.Remove(comment));

//            this.commentsService = new CommentsService(mockCommentsRepo.Object, mockVoteRepo.Object);
//        }

//        [Fact]
//        public void CommentShouldHaveTheCorrectUserId()
//        {
//            string userId = this.commentsService.GetCommentUserId(1);

//            Assert.Equal("2ce0609c-7492-47cf-a09b-14e6b329e5d7", userId);
//        }

//        [Fact]
//        public async Task DeleteCommentShouldWorkCorrectly()
//        {
//            await this.commentsService.DeleteAsync(1);

//            Comment comment = this.comments.FirstOrDefault(x => x.Id == 1);
//            Assert.Null(comment);
//        }

//        [Fact]
//        public async Task CommentCreateShouldWorkCorrectly()
//        {
//            await this.commentsService.Create("cf092d2f-e9c5-4416-a3d1-b6769cd7f364", "Some comment content", "a28c1cf8-8877-402c-adb7-855dbf5905da");

//            Comment comment = this.comments.FirstOrDefault(x =>
//                                                       x.UserId == "cf092d2f-e9c5-4416-a3d1-b6769cd7f364" &&
//                                                       x.BookId == "a28c1cf8-8877-402c-adb7-855dbf5905da" &&
//                                                       x.Content == "Some comment content");
//            Assert.Equal(3, this.comments.Count);
//            Assert.NotNull(comment);
//            Assert.Equal("cf092d2f-e9c5-4416-a3d1-b6769cd7f364", comment.UserId);
//            Assert.Equal("a28c1cf8-8877-402c-adb7-855dbf5905da", comment.BookId);
//            Assert.Equal("Some comment content", comment.Content);
//        }
//    }
//}
