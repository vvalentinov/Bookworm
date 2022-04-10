namespace Bookworm.Services.Data.Models
{
    using System.Linq;
    using System.Threading.Tasks;

    using Bookworm.Data.Common.Repositories;
    using Bookworm.Data.Models;
    using Bookworm.Services.Data.Contracts;

    public class CommentsService : ICommentsService
    {
        private readonly IDeletableEntityRepository<Comment> commentRepository;
        private readonly IRepository<Vote> voteRepository;

        public CommentsService(IDeletableEntityRepository<Comment> commentRepository, IRepository<Vote> voteRepository)
        {
            this.commentRepository = commentRepository;
            this.voteRepository = voteRepository;
        }

        public async Task Create(string userId, string content, string bookId)
        {
            var comment = new Comment()
            {
                UserId = userId,
                Content = content,
                BookId = bookId,
            };

            await this.commentRepository.AddAsync(comment);
            await this.commentRepository.SaveChangesAsync();
        }

        public async Task DeleteAsync(int commentId)
        {
            var votes = this.voteRepository.All().Where(x => x.CommentId == commentId).ToList();
            foreach (var vote in votes)
            {
                this.voteRepository.Delete(vote);
            }

            await this.voteRepository.SaveChangesAsync();

            var comments = this.commentRepository.All().Where(x => x.Id == commentId).ToList();
            foreach (var comment in comments)
            {
                this.commentRepository.Delete(comment);
            }

            await this.commentRepository.SaveChangesAsync();
        }

        public string GetCommentUserId(int commentId)
        {
            var comment = this.commentRepository
                .AllAsNoTracking()
                .FirstOrDefault(x => x.Id == commentId);

            return comment.UserId;
        }
    }
}
