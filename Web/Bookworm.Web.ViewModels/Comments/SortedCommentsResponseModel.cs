namespace Bookworm.Web.ViewModels.Comments
{
    using System.Collections.Generic;

    public class SortedCommentsResponseModel
    {
        public IEnumerable<CommentViewModel> Comments { get; set; }

        public bool IsUserSignedIn { get; set; }

        public bool IsUserAdmin { get; set; }
    }
}
