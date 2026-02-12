using AutoMapper;
using EduApp.Data;
using EduApp.Extensions;
using EduApp.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuizApp.Domain;
using QuizApp.Repositories;

namespace EduApp.Controllers
{
    [Authorize]
    public class QuizAttemptController : Controller
    {
        private readonly IQuizAttemptRepository _quizAttemptRepository;
        private readonly ILogger<QuizAttemptController> _logger;
        private readonly IMapper _mapper;

        public QuizAttemptController(IQuizAttemptRepository quizRepository, ILogger<QuizAttemptController> logger, IMapper mapper)
        {
            _quizAttemptRepository = quizRepository;
            _logger = logger;
            _mapper = mapper;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Start(int quizId)
        {
            if (User?.Identity?.IsAuthenticated != true)
            {
                return RedirectToAction("Login", "Account");
            }

            var userId = User.GetUserId();

            var attempt = new QuizAttempt
            {
                QuizId = quizId,
                UserId = userId!,
                StartedAt = DateTime.UtcNow
            };

            await _quizAttemptRepository.AddQuizAttemptAsync(attempt);

            return RedirectToAction("Question", new { attemptId = attempt.Id, index = 0 });
        }

        public async Task<IActionResult> Result(int attemptId)
        {
            //* move to service
            var attempt = await _quizAttemptRepository.GetAttemptAsync(attemptId);

            if (attempt == null)
                return NotFound();

            var userQuestionsAttempt = await _quizAttemptRepository.GetQuestionAttemptsAsync(attemptId);

            foreach (var userQuestionAttempt in userQuestionsAttempt)
            {
                attempt.QuestionAttempts.Add(userQuestionAttempt);
            }

            attempt.Score = attempt.CalculateScore();
            attempt.FinishedAt = DateTime.UtcNow;
            await _quizAttemptRepository.UpdateQuizAttemptAsync(attempt);
            //* 

            var resultDto = new QuizResultDto
            {
                Score = attempt.Score,
                Total = attempt.Quiz.Questions.Count,
                QuizId = attempt.QuizId,
            };

            return View(resultDto);
        }

        [HttpPost]
        public async Task<IActionResult> Question(QuestionDto model)
        {
            var attempt = await _quizAttemptRepository.GetAttemptAsync(model.AttemptId);
            if (attempt == null) return NotFound();

            var question = attempt?.Quiz?.Questions?.ElementAt(model.QuestionIndex);
            if (question is null) return NotFound();

            var selectedAnswer = question.Answers.ElementAt(model.SelectedAnswerIndex);

            var qa = new QuestionAttempt
            {
                QuizAttemptId = attempt!.Id,
                QuestionId = question.Id,
                SelectedAnswerId = selectedAnswer.Id
            };

            await _quizAttemptRepository.AddQuestionAttemptAsync(qa);

            if (model.QuestionIndex + 1 >= attempt.Quiz.Questions.Count)
            {
                attempt.FinishedAt = DateTime.UtcNow;
                await _quizAttemptRepository.UpdateQuizAttemptAsync(attempt);

                return RedirectToAction("Result", new { attemptId = model.AttemptId });
            }

            // Otherwise, go to next question
            return RedirectToAction("Question", new
            {
                attemptId = model.AttemptId,
                index = model.QuestionIndex + 1
            });
        }

        public async Task<IActionResult> Question(int attemptId, int index)
        {
            var attempt = await _quizAttemptRepository.GetAttemptAsync(attemptId);
            if (attempt is null) return NotFound();

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
    }
}
