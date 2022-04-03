namespace Bookworm.Web.Controllers
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Bookworm.Data.Models.Dtos;
    using Bookworm.Services.Data.Contracts;
    using Bookworm.Web.ViewModels.Quizzes;
    using Microsoft.AspNetCore.Mvc;

    public class QuizController : BaseController
    {
        private readonly IQuizService quizService;

        public QuizController(IQuizService quizService)
        {
            this.quizService = quizService;
        }

        public IActionResult TakeQuiz()
        {
            TakeQuizFormModel model = new TakeQuizFormModel()
            {
                Categories = this.quizService.GetQuizCategories(),
            };

            return this.View(model);
        }

        [HttpPost]
        public async Task<IActionResult> TakeQuiz(TakeQuizFormModel model)
        {
            List<QuizQuestion> questions = await this.quizService.GetQuizQuestionsAsync(model.CategoryName, model.CountQuestions);

            QuizViewModel quizViewModel = this.quizService.GetQuizModel(questions, model.CategoryName);

            return this.View("Quiz", quizViewModel);
        }

        [HttpPost]
        public IActionResult Quiz(QuizViewModel model)
        {
            ResultViewModel resultModel = this.quizService.CalculateResult(model.Questions);
            return this.View("QuizResult", resultModel);
        }
    }
}
