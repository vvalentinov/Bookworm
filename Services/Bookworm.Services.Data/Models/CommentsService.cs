namespace Bookworm.Services.Data.Models
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    using Bookworm.Data.Common.Repositories;
    using Bookworm.Data.Models;
    using Bookworm.Services.Data.Contracts;
    using Microsoft.EntityFrameworkCore;

    public class CommentsService : ICommentsService
    {
        private readonly IRepository<Comment> commentRepository;
        private readonly IRepository<Vote> voteRepository;

        public CommentsService(
            IRepository<Comment> commentRepository,
            IRepository<Vote> voteRepository)
        {
            this.commentRepository = commentRepository;
            this.voteRepository = voteRepository;
        }

        public async Task CreateAsync(
            string userId,
            string content,
            string bookId)
        {
            Comment comment = new Comment()
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
            Comment comment = await this.commentRepository
                .AllAsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == commentId);

            if (comment == null)
            {
                throw new InvalidOperationException("Comment with given id not found!");
            }

            // TODO: Delete comment's votes from the Votes table
            this.commentRepository.Delete(comment);
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
