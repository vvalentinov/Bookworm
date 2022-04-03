namespace Bookworm.Services.Data.Contracts
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Bookworm.Data.Models.Dtos;
    using Bookworm.Web.ViewModels.Quizzes;
    using Microsoft.AspNetCore.Mvc.Rendering;

    public interface IQuizService
    {
        IEnumerable<SelectListItem> GetQuizCategories();

        Task<List<QuizQuestion>> GetQuizQuestionsAsync(string categoryName, int countQuestions);

        QuizViewModel GetQuizModel(List<QuizQuestion> questions, string categoryName);

        ResultViewModel CalculateResult(IList<QuizQuestionViewModel> questions);
    }
}
