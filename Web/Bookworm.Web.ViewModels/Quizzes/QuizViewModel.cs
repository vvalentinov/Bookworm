namespace Bookworm.Web.ViewModels.Quizzes
{
    using System.Collections.Generic;

    public class QuizViewModel
    {
        public string Category { get; set; }

        public IList<QuizQuestionViewModel> Questions { get; set; }
    }
}
