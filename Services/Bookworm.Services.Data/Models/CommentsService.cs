namespace Bookworm.Services.Data.Models
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    using Bookworm.Common;
    using Bookworm.Data.Common.Repositories;
    using Bookworm.Data.Models;
    using Bookworm.Services.Data.Contracts;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.EntityFrameworkCore;

    public class CommentsService : ICommentsService
    {
        private readonly IRepository<Comment> commentRepository;
        private readonly IRepository<Vote> voteRepository;
        private readonly IDeletableEntityRepository<ApplicationUser> userRepository;
        private readonly UserManager<ApplicationUser> userManager;

        public CommentsService(
            IRepository<Comment> commentRepository,
            IRepository<Vote> voteRepository,
            IDeletableEntityRepository<ApplicationUser> userRepository,
            UserManager<ApplicationUser> userManager)
        {
            this.commentRepository = commentRepository;
            this.voteRepository = voteRepository;
            this.userRepository = userRepository;
            this.userManager = userManager;
        }

        public async Task CreateAsync(
            string userId,
            string content,
            string bookId)
        {
            if (string.IsNullOrWhiteSpace(content))
            {
                throw new InvalidOperationException("Comment's content must not be empty!");
            }

            Comment comment = new Comment()
            {
                UserId = userId,
                Content = content,
                BookId = bookId,
            };

            await this.commentRepository.AddAsync(comment);
            await this.commentRepository.SaveChangesAsync();
        }

        public async Task EditAsync(int commentId, string content, string userId)
        {
            if (string.IsNullOrWhiteSpace(content))
            {
                throw new InvalidOperationException("Comment's content must not be empty!");
            }

            Comment comment = await this.commentRepository
                .AllAsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == commentId);

            if (comment == null)
            {
                throw new InvalidOperationException("Comment with given id not found!");
            }

            ApplicationUser user = await this.userRepository.AllAsNoTracking().FirstOrDefaultAsync(x => x.Id == userId);

            if (user == null)
            {
                throw new InvalidOperationException("No user with given id found!");
            }

            bool isAdmin = await this.userManager.IsInRoleAsync(user, GlobalConstants.AdministratorRoleName);

            if (!isAdmin || comment.UserId != userId)
            {
                throw new InvalidOperationException("You have to be either the comment's author or an administrator to edit it!");
            }

            comment.Content = content;
            this.commentRepository.Update(comment);
            await this.commentRepository.SaveChangesAsync();
        }

        public async Task DeleteAsync(int commentId, string userId)
        {
            Comment comment = await this.commentRepository
                .AllAsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == commentId);

            if (comment == null)
            {
                throw new InvalidOperationException("Comment with given id not found!");
            }

            ApplicationUser user = await this.userRepository.AllAsNoTracking().FirstOrDefaultAsync(x => x.Id == userId);
            bool isAdmin = await this.userManager.IsInRoleAsync(user, GlobalConstants.AdministratorRoleName);

            if (!isAdmin || comment.UserId != userId)
            {
                throw new InvalidOperationException("You have to be either the comment's author or an administrator to delete!");
            }

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
