namespace Bookworm.Web.ViewModels.Pagination
{
    public abstract class BasePaginationViewModel : PaginationViewModel
    {
        public PaginationNavigationViewModel PaginationNavigation { get; set; }
    }
}
