namespace Bookworm.Web.Controllers
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Bookworm.Common;
    using Bookworm.Data.Models;
    using Bookworm.Data.Models.Dtos;
    using Bookworm.Services.Data.Contracts;
    using Bookworm.Web.ViewModels.Quizzes;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;

    public class QuizController : BaseController
    {
        private readonly IQuizService quizService;
        private readonly UserManager<ApplicationUser> userManager;

        public QuizController(IQuizService quizService, UserManager<ApplicationUser> userManager)
        {
            this.quizService = quizService;
            this.userManager = userManager;
        }

        [Authorize]
        public IActionResult TakeQuiz()
        {
            TakeQuizFormModel model = new()
            {
                Categories = this.quizService.GetQuizCategories(),
            };

            return this.View(model);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> TakeQuiz(TakeQuizFormModel model)
        {
            List<QuizQuestion> questions = await this.quizService.GetQuizQuestionsAsync(model.CategoryName, model.CountQuestions);

            QuizViewModel quizViewModel = this.quizService.GetQuizModel(questions, model.CategoryName);

            return this.View("Quiz", quizViewModel);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Quiz(QuizViewModel model)
        {
            ApplicationUser user = await this.userManager.GetUserAsync(this.User);
            ResultViewModel resultModel = this.quizService.CalculateResult(model.Questions);
            user.Points += resultModel.Result;
            await this.userManager.UpdateAsync(user);
            this.TempData[MessageConstant.SuccessMessage] = $"+{resultModel.Result} points";
            return this.View("QuizResult", resultModel);
        }
    }
}
