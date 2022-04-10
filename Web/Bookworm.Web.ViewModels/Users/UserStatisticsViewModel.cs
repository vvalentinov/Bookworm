namespace Bookworm.Web.ViewModels.Users
{
    public class UserStatisticsViewModel
    {
        public string Id { get; set; }

        public string UserName { get; set; }

        public int Points { get; set; }

        public int UploadedBooks { get; set; }

        public int UploadedQuotes { get; set; }
    }
}
