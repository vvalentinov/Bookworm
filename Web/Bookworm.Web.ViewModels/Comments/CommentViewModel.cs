namespace Bookworm.Web.ViewModels.Comments
{
    using System;
    using System.Collections.Generic;

    using Bookworm.Data.Models;
    using Bookworm.Services.Mapping;
    using Ganss.Xss;

    public class CommentViewModel : IMapFrom<Comment>
    {
        public int Id { get; set; }

        public DateTime CreatedOn { get; set; }

        public string Content { get; set; }

        public int NetWorth { get; set; }

        public string UserUserName { get; set; }

        public string UserId { get; set; }

        public int UserVoteValue { get; set; }

        public bool IsCommentOwner { get; set; }

        public string SanitizedContent => new HtmlSanitizer().Sanitize(this.Content);

        public ICollection<Vote> Votes { get; set; }
    }
}
