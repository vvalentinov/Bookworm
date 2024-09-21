namespace Bookworm.Web.ViewModels.Comments
{
    public class SortedCommentsResponseModel : CommentsListingViewModel
    {
        public bool IsUserAdmin { get; set; }

        public bool IsUserSignedIn { get; set; }
    }
}
