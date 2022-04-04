namespace Bookworm.Web.ViewModels.Quizzes
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    using Microsoft.AspNetCore.Mvc.Rendering;

    public class TakeQuizFormModel
    {
        public int CountQuestions { get; set; }

        [Required(ErrorMessage = "Please select category!")]
        public string CategoryName { get; set; }

        public IEnumerable<SelectListItem> Categories { get; set; }
    }
}
