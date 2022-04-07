namespace Bookworm.Services.Data.Tests
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;

    using Bookworm.Services.Data.Models;
    using Bookworm.Web.ViewModels.Quizzes;
    using Microsoft.Extensions.Configuration;
    using Xunit;

    public class QuizServiceTests
    {
        private readonly QuizService quizService;

        public QuizServiceTests()
        {
            this.Configuration = this.SetConfiguration();
            this.quizService = new QuizService(this.Configuration);
        }

        protected IConfigurationRoot Configuration { get; set; }

        [Fact]
        public void GetQuizCategoriesShouldWorkCorrectly()
        {
            var categories = this.quizService.GetQuizCategories().ToList();

            Assert.Equal(9, categories.Count);

            Assert.Equal("Film & TV", categories[0].Text);
            Assert.Equal("Film & TV", categories[0].Value);

            Assert.Equal("Food & Drink", categories[1].Text);
            Assert.Equal("Food & Drink", categories[1].Value);

            Assert.Equal("General Knowledge", categories[2].Text);
            Assert.Equal("General Knowledge", categories[2].Value);

            Assert.Equal("Geography", categories[3].Text);
            Assert.Equal("Geography", categories[3].Value);

            Assert.Equal("History", categories[4].Text);
            Assert.Equal("History", categories[4].Value);

            Assert.Equal("Music", categories[5].Text);
            Assert.Equal("Music", categories[5].Value);

            Assert.Equal("Science", categories[6].Text);
            Assert.Equal("Science", categories[6].Value);

            Assert.Equal("Society & Culture", categories[7].Text);
            Assert.Equal("Society & Culture", categories[7].Value);

            Assert.Equal("Sport & Leisure", categories[8].Text);
            Assert.Equal("Sport & Leisure", categories[8].Value);
        }

        [Fact]
        public async Task GetQuizQuestionsShouldWorkCorrectly()
        {
            var questions = await this.quizService.GetQuizQuestionsAsync("History", 5);

            Assert.Equal(5, questions.Count);
        }

        [Fact]
        public async Task GetQuizModelShouldWorkCorrectly()
        {
            var questions = await this.quizService.GetQuizQuestionsAsync("History", 5);
            var model = this.quizService.GetQuizModel(questions, "History");

            Assert.Equal("History", model.Category);
            Assert.Equal(5, model.Questions.Count());
        }

        [Fact]
        public void CalculateResultShouldWorkCorrectly()
        {
            List<QuizQuestionViewModel> questions = new List<QuizQuestionViewModel>()
            {
                new QuizQuestionViewModel()
                {
                     QuestionName = "What is the nickname of the English football team Barnsley?",
                     SelectedAnswer = "Wolves",
                     CorrectAnswer = "The Tykes",
                },
                new QuizQuestionViewModel()
                {
                     QuestionName = "What Is Hydrophobia Better Known As?",
                     SelectedAnswer = "Rabies",
                     CorrectAnswer = "Rabies",
                },
                new QuizQuestionViewModel()
                {
                     QuestionName = "Which philosopher famously said 'The only thing I know is that I know nothing'?",
                     SelectedAnswer = "Socrates",
                     CorrectAnswer = "Socrates",
                },
                new QuizQuestionViewModel()
                {
                     QuestionName = "What is the capital city of Uganda?",
                     SelectedAnswer = "Lilongwe",
                     CorrectAnswer = "Kampala",
                },
                new QuizQuestionViewModel()
                {
                     QuestionName = "Which band includes 'David Coverdale'?",
                     SelectedAnswer = "Blur",
                     CorrectAnswer = "Whitesnake",
                },
            };

            var model = this.quizService.CalculateResult(questions);

            Assert.Equal(2, model.Result);
            Assert.Equal(5, model.NumberOfQuestions);
            Assert.Equal(3, model.IncorrectAnswers.Count());
        }

        private IConfigurationRoot SetConfiguration()
        {
            return new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile(
                 path: "appsettings.json",
                 optional: false,
                 reloadOnChange: true)
           .Build();
        }
    }
}
