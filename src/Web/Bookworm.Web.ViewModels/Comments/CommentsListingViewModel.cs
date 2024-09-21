namespace Bookworm.Web.ViewModels.Comments
{
    using System.Collections.Generic;

    public class CommentsListingViewModel : BaseListingViewModel
    {
        public IEnumerable<CommentViewModel> Comments { get; set; }
    }
}
