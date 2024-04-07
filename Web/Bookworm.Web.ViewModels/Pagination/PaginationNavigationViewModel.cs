namespace Bookworm.Web.ViewModels.Pagination
{
    public class PaginationNavigationViewModel
    {
        public string PaginationAction { get; set; }

        public string PaginationController { get; set; }

        public bool IsPaginationForBooks { get; set; }
    }
}
