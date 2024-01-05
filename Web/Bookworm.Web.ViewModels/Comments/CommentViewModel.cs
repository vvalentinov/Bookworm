namespace Bookworm.Web.ViewModels.Comments
{
    using System;
    using System.Linq;

    using AutoMapper;
    using Bookworm.Data.Models;
    using Bookworm.Services.Mapping;
    using Ganss.Xss;

    public class CommentViewModel : IMapFrom<Comment>, IHaveCustomMappings
    {
        public int Id { get; set; }

        public DateTime CreatedOn { get; set; }

        public string Content { get; set; }

        public int NetWorth { get; set; }

        public string SanitizedContent => new HtmlSanitizer().Sanitize(this.Content);

        public string UserUserName { get; set; }

        public string UserId { get; set; }

        public int VoteValue { get; set; }

        public void CreateMappings(IProfileExpression configuration)
        {
            configuration.CreateMap<Comment, CommentViewModel>()
                .ForMember(x => x.VoteValue, opt =>
                    opt.MapFrom(x =>
                        x.Votes.FirstOrDefault() == null ? 0 : x.Votes.FirstOrDefault().Value));
        }
    }
}
