namespace Bookworm.Services.Data.Tests
{
    using System;
    using System.Linq;
    using System.Net;
    using System.Threading.Tasks;

    using Bookworm.Data;
    using Bookworm.Data.Models;
    using Bookworm.Data.Repositories;
    using Bookworm.Services.Data.Models;
    using Bookworm.Services.Data.Tests.Shared;
    using Microsoft.EntityFrameworkCore;
    using Xunit;

    using static Bookworm.Common.Constants.ErrorMessagesConstants.BookErrorMessagesConstants;
    using static Bookworm.Common.Constants.ErrorMessagesConstants.CommentErrorMessagesConstants;
    using static Bookworm.Common.Enums.SortCommentsCriteria;

    public class CommentsServiceTests : IClassFixture<DbContextFixture>
    {
        private readonly ApplicationDbContext dbContext;

        public CommentsServiceTests(DbContextFixture dbContextFixture)
        {
            this.dbContext = dbContextFixture.DbContext;
        }

        [Fact]
        public async Task CreateShouldWorkCorrectly()
        {
            var service = this.GetCommentsService();
            var commentRepo = this.GetCommentRepo();

            var content = "Some comment content";
            var userId = "0fc3ea28-3165-440e-947e-670c90562320";

            await service.CreateAsync(userId, content, 4);

            var comment = await commentRepo.AllAsNoTracking().FirstOrDefaultAsync(c => c.Content == content);

            Assert.NotNull(comment);
            Assert.Equal(userId, comment.UserId);
            Assert.Equal(0, comment.NetWorth);
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        [InlineData("   ")]
        public async Task CreateShouldThrowExceptionIfContentIsNullOrWhiteSpace(string content)
        {
            var service = this.GetCommentsService();

            var exception = await Assert.ThrowsAsync<InvalidOperationException>(async ()
                => await service.CreateAsync(string.Empty, content, 1));

            Assert.Equal(CommentContentEmptyError, exception.Message);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-5)]
        [InlineData(11)]
        public async Task CreateShouldThrowExceptionIfBookIdIsInvalid(int bookId)
        {
            var service = this.GetCommentsService();

            var exception = await Assert.ThrowsAsync<InvalidOperationException>(async ()
                => await service.CreateAsync(string.Empty, "Some Content", bookId));

            Assert.Equal(BookWrongIdError, exception.Message);
        }

        [Theory]
        [InlineData("0fc3ea28-3165-440e-947e-670c90562320", true)]
        [InlineData("f19d077c-ceb8-4fe2-b369-45abd5ffa8f7", false)]
        public async Task EditShouldWorkCorrectly(string userId, bool isAdmin)
        {
            var repo = this.GetCommentRepo();
            var service = this.GetCommentsService();

            await service.EditAsync(1, "Updated Content", userId, isAdmin);

            var comment = await repo.AllAsNoTracking().FirstOrDefaultAsync(c => c.Id == 1);

            Assert.NotNull(comment);
            Assert.Equal("Updated Content", comment.Content);
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        [InlineData("   ")]
        public async Task EditShouldThrowExceptionIfContentIsNullOrWhiteSpace(string content)
        {
            var service = this.GetCommentsService();

            var exception = await Assert.ThrowsAsync<InvalidOperationException>(async ()
                => await service.EditAsync(1, content, string.Empty, false));

            Assert.Equal(CommentContentEmptyError, exception.Message);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(7)]
        [InlineData(-12)]
        public async Task EditShouldThrowExceptionIfCommentIdIsInvalid(int id)
        {
            var service = this.GetCommentsService();

            var exception = await Assert.ThrowsAsync<InvalidOperationException>(async ()
                => await service.EditAsync(id, "Some Content Here", string.Empty, false));

            Assert.Equal(CommentWrongIdError, exception.Message);
        }

        [Fact]
        public async Task EditShouldThrowExceptionIfUserIsNotAdminOrCommentCreator()
        {
            var service = this.GetCommentsService();

            var exception = await Assert.ThrowsAsync<InvalidOperationException>(async ()
                => await service.EditAsync(1, "Some Content Here", "a84ea5dc-a89e-442f-8e53-c874675bb114", false));

            Assert.Equal(CommentEditError, exception.Message);
        }

        [Fact]
        public async Task DeleteShouldWorkCorrectly()
        {
            var voteRepo = this.GetVoteRepo();
            var service = this.GetCommentsService();
            var commentRepo = this.GetCommentRepo();

            var userId = "f19d077c-ceb8-4fe2-b369-45abd5ffa8f7";
            var commentId = 4;

            await service.DeleteAsync(commentId, userId, false);

            var votesCount = await voteRepo.AllAsNoTracking().Where(v => v.CommentId == commentId).CountAsync();
            var comment = await commentRepo.AllAsNoTrackingWithDeleted().FirstOrDefaultAsync(c => c.Id == commentId);

            Assert.NotNull(comment);
            Assert.True(comment.IsDeleted);
            Assert.Equal(0, votesCount);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(6)]
        [InlineData(-14)]
        public async Task DeleteShouldThrowExceptionIfIdIsInvalid(int id)
        {
            var service = this.GetCommentsService();

            var exception = await Assert.ThrowsAsync<InvalidOperationException>(async ()
                => await service.DeleteAsync(id, "Some UserID Here", false));

            Assert.Equal(CommentWrongIdError, exception.Message);
        }

        [Fact]
        public async Task DeleteShouldThrowExceptionIfUserIsNotAdminOrCommentCreator()
        {
            var service = this.GetCommentsService();

            var exception = await Assert.ThrowsAsync<InvalidOperationException>(async ()
                => await service.DeleteAsync(1, "a84ea5dc-a89e-442f-8e53-c874675bb114", false));

            Assert.Equal(CommentDeleteError, exception.Message);
        }

        [Theory]
        [InlineData(nameof(CreatedOnAsc))]
        [InlineData(nameof(NetWorthDesc))]
        [InlineData(nameof(CreatedOnDesc))]
        public async Task GetSortedCommentsShouldWorkCorrectly(string sortCriteria)
        {
            var service = this.GetCommentsService();
            var userId = "a84ea5dc-a89e-442f-8e53-c874675bb114";

            var model = await service.GetSortedCommentsAsync(1, userId, sortCriteria, false);

            var comments = model.Comments.ToList();

            switch (sortCriteria)
            {
                case nameof(CreatedOnAsc):
                    Assert.Equal(1, comments[0].Id);
                    Assert.Equal(2, comments[1].Id);

                    Assert.False(comments[0].IsCommentOwner);
                    Assert.True(comments[1].IsCommentOwner);

                    Assert.Equal(1, comments[0].UserVoteValue);
                    Assert.Equal(0, comments[1].UserVoteValue);
                    break;
                case nameof(NetWorthDesc):
                    Assert.Equal(1, comments[0].Id);
                    Assert.Equal(2, comments[1].Id);

                    Assert.False(comments[0].IsCommentOwner);
                    Assert.True(comments[1].IsCommentOwner);

                    Assert.Equal(1, comments[0].UserVoteValue);
                    Assert.Equal(0, comments[1].UserVoteValue);
                    break;
                case nameof(CreatedOnDesc):
                    Assert.Equal(2, comments[0].Id);
                    Assert.Equal(1, comments[1].Id);

                    Assert.True(comments[0].IsCommentOwner);
                    Assert.False(comments[1].IsCommentOwner);

                    Assert.Equal(0, comments[0].UserVoteValue);
                    Assert.Equal(1, comments[1].UserVoteValue);
                    break;
            }
        }

        [Fact]
        public async Task GetSortedCommentsShouldThrowExceptionIfBookIdIsInvalid()
        {
            var service = this.GetCommentsService();

            var exception = await Assert.ThrowsAsync<InvalidOperationException>(async ()
                => await service.GetSortedCommentsAsync(11, string.Empty, string.Empty, false));

            Assert.Equal(BookWrongIdError, exception.Message);
        }

        [Fact]
        public async Task GetSortedCommentsShouldThrowExceptionIfSortCriteriaIsInvalid()
        {
            var service = this.GetCommentsService();

            var exception = await Assert.ThrowsAsync<InvalidOperationException>(async ()
                => await service.GetSortedCommentsAsync(1, string.Empty, "Invalid Criteria", false));

            Assert.Equal(CommentInvalidSortCriteria, exception.Message);
        }

        private EfRepository<Vote> GetVoteRepo() => new(this.dbContext);

        private EfDeletableEntityRepository<Comment> GetCommentRepo() => new(this.dbContext);

        private EfDeletableEntityRepository<Book> GetBookRepo() => new(this.dbContext);

        private CommentsService GetCommentsService()
            => new (this.GetVoteRepo(), this.GetCommentRepo(), this.GetBookRepo());
    }
}
