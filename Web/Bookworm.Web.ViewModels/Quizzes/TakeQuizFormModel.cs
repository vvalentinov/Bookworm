namespace Bookworm.Web.ViewModels.Quizzes
{
    using System.Collections.Generic;

    using Microsoft.AspNetCore.Mvc.Rendering;

    public class TakeQuizFormModel
    {
        public int CountQuestions { get; set; }

        public string CategoryName { get; set; }

        public IEnumerable<SelectListItem> Categories { get; set; }
    }
}
