using AutoMapper;
using EduApp.Data;
using EduApp.Models;
using EduApp.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuizApp.Domain;
using System.Security.Claims;

namespace EduApp.Controllers
{
    [Authorize]
    public class QuizController : Controller
    {
        private readonly IQuizRepository _quizRepository;
        private readonly IMapper _mapper;

        public QuizController(
            IQuizRepository quizRepository,
            IMapper mapper)
        {
            _quizRepository = quizRepository;
            _mapper = mapper;
        }

        public IActionResult Index()
        {
            var quizzes = _quizRepository.GetAllQuizzes();
            return View(quizzes.ToList());
        }

        public IActionResult SelectedQuiz(int id)
        {
            var quiz = _quizRepository.GetQuizById(id);
            if (quiz == null) return NotFound();

            return View(quiz);
        }

        public IActionResult Start(int quizId)
        {
            if (User?.Identity?.IsAuthenticated != true)
            {
                return RedirectToAction("Login", "Account");
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var attempt = new QuizAttempt
            {
                QuizId = quizId,
                UserId = userId!,
                StartedAt = DateTime.UtcNow
            };

            _quizRepository.AddQuizAttempt(attempt);

            return RedirectToAction("Question", new { attemptId = attempt.Id, index = 0 });
        }

        public IActionResult Question(int attemptId, int index)
        {
            var attempt = _quizRepository.GetAttempt(attemptId);
             
            var question = attempt.Quiz.Questions.ElementAt(index);

            var questionDto = new QuestionDto
            {
                AttemptId = attemptId,
                QuizId = attempt.QuizId,
                QuestionIndex = index,
                Text = question.Text,
                Answers = question.Answers.Select(a => _mapper.Map<AnswerDto>(a)).ToList()
            };

            return View(questionDto);
        }

        [HttpPost]
        public IActionResult Question(QuestionDto model)
        {
            var attempt = _quizRepository.GetAttempt(model.AttemptId);

            var question = attempt.Quiz.Questions.ElementAt(model.QuestionIndex);
            var selectedAnswer = question.Answers.ElementAt(model.SelectedAnswerIndex);

            var qa = new QuestionAttempt
            {
                QuizAttemptId = attempt.Id,
                QuestionId = question.Id,
                SelectedAnswerId = selectedAnswer.Id
            };

            _quizRepository.AddQuestionAttempt(qa);

            if (model.QuestionIndex + 1 >= attempt.Quiz.Questions.Count)
            {
                attempt.FinishedAt = DateTime.UtcNow;
                _quizRepository.UpdateQuizAttempt(attempt);

                return RedirectToAction("Result", new { attemptId = model.AttemptId });
            }

            // Otherwise, go to next question
            return RedirectToAction("Question", new
            {
                attemptId = model.AttemptId,
                index = model.QuestionIndex + 1
            });
        }

        public IActionResult Result(int attemptId)
        {
            // Load the attempt including questions and answers
            var attempt = _quizRepository.GetAttempt(attemptId);

            if (attempt == null)
                return NotFound();

            // Load all answers the user selected
            var userAnswers = _quizRepository.GetQuestionAttempts(attemptId);

            // Calculate score
            int score = 0;

            foreach (var question in attempt.Quiz.Questions)
            {
                var userAnswer = userAnswers.FirstOrDefault(qa => qa.QuestionId == question.Id);
                if (userAnswer != null)
                {
                    var selected = question.Answers.First(a => a.Id == userAnswer.SelectedAnswerId);
                    if (selected.IsCorrect)
                        score++;
                }
            }

            var resultVm = new QuizResultViewModel
            {
                Score = score,
                Total = attempt.Quiz.Questions.Count,
            };

            return View(resultVm);
        }

        public IActionResult Create()
        {
            var quiz = new QuizDto();

            //for (int i = 0; i < 3; i++) // 3 questions
            //{
            //    var question = new QuestionDto();
            //    for (int j = 0; j < 2; j++) // 4 answers per question
            //    {
            //        question.Answers.Add(new AnswerDto());
            //    }
            //    quiz.Questions.Add(question);
            //}

            return View(quiz);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(QuizDto quizDto, [FromForm] int[] correctAnswers)
        {
            if (!ModelState.IsValid)
                return View(quizDto); // redisplay form with validation messages

            var quiz = _mapper.Map<Quiz>(quizDto);

            // 1️⃣ Set Quiz reference for each question
            for (int i = 0; i < quiz.Questions.Count; i++)
            {
                var question = quiz.Questions[i];
                question.Quiz = quiz; // EF Core will set Question.QuizId automatically

                // 2️⃣ Set correct answer
                if (correctAnswers.Length <= i)
                    continue; // skip if missing

                int correctIndex = correctAnswers[i];

                if (correctIndex >= 0 && correctIndex < quiz.Questions[i].Answers.Count)
                {
                    quiz.Questions[i].Answers[correctIndex].IsCorrect = true;
                }
            }

            _quizRepository.AddQuiz(quiz);
            return RedirectToAction(nameof(Index)); // View(quiz);
        }

        public IActionResult Edit(int id)
        {
            var quiz = _quizRepository.GetQuizById(id);
            if (quiz == null) return NotFound();

            return View(quiz);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Quiz quiz)
        {
            if (ModelState.IsValid)
            {
                _quizRepository.UpdateQuiz(quiz);
                return RedirectToAction(nameof(Index));
            }
            return View(quiz);
        }

        public IActionResult Delete(int id)
        {
            var quiz = _quizRepository.GetQuizById(id);
            if (quiz == null) return NotFound();

            return View(quiz);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            _quizRepository.DeleteQuiz(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
