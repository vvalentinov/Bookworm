namespace Bookworm.Data.Models.Dtos
{
    using System.Collections.Generic;

    public class QuizQuestion
    {
        public string Question { get; set; }

        public string CorrectAnswer { get; set; }

        public IList<string> IncorrectAnswers { get; set; }
    }
}
