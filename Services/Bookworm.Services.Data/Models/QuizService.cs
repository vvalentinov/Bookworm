namespace Bookworm.Services.Data.Models
{
    using System;
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Threading.Tasks;

    using Bookworm.Data.Models.Dtos;
    using Bookworm.Services.Data.Contracts;
    using Bookworm.Web.ViewModels.Quizzes;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.Extensions.Configuration;
    using Newtonsoft.Json;

    public class QuizService : IQuizService
    {
        private readonly IConfiguration configuration;

        public QuizService(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public ResultViewModel CalculateResult(IList<QuizQuestionViewModel> questions)
        {
            int countPoints = 0;
            List<QuizQuestionViewModel> incorrectAnswers = new List<QuizQuestionViewModel>();

            foreach (var question in questions)
            {
                var selectedAnswer = question.SelectedAnswer;
                var correctAnswer = question.CorrectAnswer;
                var questionName = question.QuestionName;
                if (selectedAnswer == correctAnswer)
                {
                    countPoints++;
                }
                else
                {
                    incorrectAnswers.Add(new QuizQuestionViewModel()
                    {
                        CorrectAnswer = correctAnswer,
                        SelectedAnswer = selectedAnswer,
                        QuestionName = questionName,
                    });
                }
            }

            return new ResultViewModel()
            {
                Result = countPoints,
                NumberOfQuestions = questions.Count,
                IncorrectAnswers = incorrectAnswers,
            };
        }

        public IEnumerable<SelectListItem> GetQuizCategories()
        {
            List<string> categories = new List<string>()
            {
                "Film & TV",
                "Food & Drink",
                "General Knowledge",
                "Geography",
                "History",
                "Music",
                "Science",
                "Society & Culture",
                "Sport & Leisure",
            };
            List<SelectListItem> categoriesResult = new List<SelectListItem>();
            foreach (var category in categories)
            {
                SelectListItem selectListItem = new SelectListItem()
                {
                    Text = category,
                    Value = category,
                };

                categoriesResult.Add(selectListItem);
            }

            return categoriesResult;
        }

        public QuizViewModel GetQuizModel(List<QuizQuestion> questions, string categoryName)
        {
            List<QuizQuestionViewModel> modelQuestions = new List<QuizQuestionViewModel>();
            Random r = new Random();
            foreach (QuizQuestion question in questions)
            {
                var num = r.Next(1, 5);
                QuizQuestionViewModel quizQuestionViewModel = new QuizQuestionViewModel()
                {
                    QuestionName = question.Question,
                    CorrectAnswer = question.CorrectAnswer,
                };

                switch (num)
                {
                    case 1:
                        quizQuestionViewModel.FirstOption = question.CorrectAnswer;
                        quizQuestionViewModel.SecondOption = question.IncorrectAnswers[0];
                        quizQuestionViewModel.ThirdOption = question.IncorrectAnswers[1];
                        quizQuestionViewModel.FourthOption = question.IncorrectAnswers[2];
                        break;
                    case 2:
                        quizQuestionViewModel.FirstOption = question.IncorrectAnswers[0];
                        quizQuestionViewModel.SecondOption = question.CorrectAnswer;
                        quizQuestionViewModel.ThirdOption = question.IncorrectAnswers[1];
                        quizQuestionViewModel.FourthOption = question.IncorrectAnswers[2];
                        break;
                    case 3:
                        quizQuestionViewModel.FirstOption = question.IncorrectAnswers[0];
                        quizQuestionViewModel.SecondOption = question.IncorrectAnswers[1];
                        quizQuestionViewModel.ThirdOption = question.CorrectAnswer;
                        quizQuestionViewModel.FourthOption = question.IncorrectAnswers[2];
                        break;
                    case 4:
                        quizQuestionViewModel.FirstOption = question.IncorrectAnswers[0];
                        quizQuestionViewModel.SecondOption = question.IncorrectAnswers[1];
                        quizQuestionViewModel.ThirdOption = question.IncorrectAnswers[2];
                        quizQuestionViewModel.FourthOption = question.CorrectAnswer;
                        break;
                }

                modelQuestions.Add(quizQuestionViewModel);
            }

            QuizViewModel quizViewModel = new QuizViewModel()
            {
                Category = categoryName,
                Questions = modelQuestions,
            };

            return quizViewModel;
        }

        public async Task<List<QuizQuestion>> GetQuizQuestionsAsync(string categoryName, int countQuestions)
        {
            List<QuizQuestion> questionList = new List<QuizQuestion>();
            string category = this.configuration.GetValue<string>($"QuizCategories:{categoryName}");
            using (HttpClient httpClient = new HttpClient())
            {
                using (HttpResponseMessage response = await httpClient.GetAsync($"https://the-trivia-api.com/questions?categories={category}&limit={countQuestions}"))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    questionList = JsonConvert.DeserializeObject<List<QuizQuestion>>(apiResponse);
                }
            }

            return questionList;
        }
    }
}
