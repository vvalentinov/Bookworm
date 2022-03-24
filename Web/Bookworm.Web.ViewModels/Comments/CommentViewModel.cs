namespace Bookworm.Web.ViewModels.Comments
{
    using System;

    using Bookworm.Data.Models;
    using Bookworm.Services.Mapping;
    using Ganss.XSS;

    public class CommentViewModel : IMapFrom<Comment>
    {
        public int Id { get; set; }

        public DateTime CreatedOn { get; set; }

        public string Content { get; set; }

        public string SanitizedContent => new HtmlSanitizer().Sanitize(this.Content);

        public string UserUserName { get; set; }
    }
}
