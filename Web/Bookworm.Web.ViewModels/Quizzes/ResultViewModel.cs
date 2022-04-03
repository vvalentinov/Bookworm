namespace Bookworm.Web.ViewModels.Quizzes
{
    using System.Collections.Generic;

    public class ResultViewModel
    {
        public int Result { get; set; }

        public int NumberOfQuestions { get; set; }

        public IEnumerable<QuizQuestionViewModel> IncorrectAnswers { get; set; }
    }
}
