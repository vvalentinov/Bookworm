namespace Bookworm.Web.ViewModels.Books
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    using Bookworm.Web.ViewModels.Categories;

    public class RandomBookFormViewModel
    {
        [Required(ErrorMessage = "Please select category!")]
        public string CategoryName { get; set; }

        public IEnumerable<CategoryViewModel> Categories { get; set; }

        public int CountBooks { get; set; }
    }
}
