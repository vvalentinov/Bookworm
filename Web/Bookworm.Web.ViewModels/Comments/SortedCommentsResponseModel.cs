namespace Bookworm.Web.ViewModels.Comments
{
    using System.Collections.Generic;

    public class SortedCommentsResponseModel
    {
        public bool IsUserAdmin { get; set; }

        public bool IsUserSignedIn { get; set; }

        public IEnumerable<CommentViewModel> Comments { get; set; }
    }
}
