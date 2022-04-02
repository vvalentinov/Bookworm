namespace Bookworm.Web.ViewModels.Books
{
    using System.Collections.Generic;

    using Microsoft.AspNetCore.Mvc.Rendering;

    public class RandomBookFormViewModel
    {
        public string CategoryName { get; set; }

        public IEnumerable<SelectListItem> Categories { get; set; }

        public int CountBooks { get; set; }
    }
}
