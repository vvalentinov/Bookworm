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

    public class CommentsServiceTests
    {
        [Fact]
        public async Task CommentShouldHaveTheCorrectUserId()
        {
            List<Comment> commentsList = new List<Comment>();
            Mock<IDeletableEntityRepository<Comment>> mockRepo = new Mock<IDeletableEntityRepository<Comment>>();
            mockRepo.Setup(x => x.AllAsNoTracking()).Returns(commentsList.AsQueryable());
            mockRepo.Setup(x => x.AddAsync(It.IsAny<Comment>()))
                .Callback((Comment comment) => commentsList.Add(comment));

            CommentsService service = new CommentsService(mockRepo.Object);

            await service.Create("1", "Some content", "someBookId");

            var result = service.GetCommentUserId(0);

            Assert.Equal("1", result);
        }
    }
}
