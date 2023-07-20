namespace Bookworm.Web.ViewModels.Comments
{
    using System;

    using Bookworm.Data.Models;
    using Bookworm.Services.Mapping;
    using Ganss.Xss;

    public class CommentViewModel : IMapFrom<Comment>
    {
        public int Id { get; set; }

        public DateTime CreatedOn { get; set; }

        public string Content { get; set; }

        public string SanitizedContent => new HtmlSanitizer().Sanitize(this.Content);

        public string UserUserName { get; set; }

        public int? UserVote { get; set; }

        public string UserId { get; set; }

        public string UserProfilePictureUrl { get; set; }

        public int UpVotesCount { get; set; }

        public int DownVotesCount { get; set; }
    }
}
