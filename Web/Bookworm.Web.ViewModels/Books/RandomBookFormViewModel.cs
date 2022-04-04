namespace Bookworm.Web.ViewModels.Books
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    using Microsoft.AspNetCore.Mvc.Rendering;

    public class RandomBookFormViewModel
    {
        [Required(ErrorMessage = "Please select category!")]
        public string CategoryName { get; set; }

        public IEnumerable<SelectListItem> Categories { get; set; }

        public int CountBooks { get; set; }
    }
}
