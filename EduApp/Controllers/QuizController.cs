using AutoMapper;
using EduApp.Extensions;
using EduApp.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuizApp.Domain;
using QuizApp.Repositories;

namespace EduApp.Controllers
{
    [Authorize]
    public class QuizController : Controller
    {
        private readonly IQuizRepository _quizRepository;
        private readonly IQuizAttemptRepository _quizAttemptRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<QuizController> _logger;

        public QuizController(
            IQuizRepository quizRepository,
            IMapper mapper,
            ILogger<QuizController> logger,
            IQuizAttemptRepository quizAttemptRepository)
        {
            _quizRepository = quizRepository;
            _mapper = mapper;
            _logger = logger;
            _quizAttemptRepository = quizAttemptRepository;
        }

        public async Task<IActionResult> Index()
        {
            var quizzes = _quizRepository.GetAllQuizzesAsync();
            return View(await quizzes);
        }

        public async Task<IActionResult> SelectedQuiz(int id)
        {
            var quiz = await _quizRepository.GetQuizByIdAsync(id);
            if (quiz == null) return NotFound();

            return View(quiz);
        }


        // quiz attempt controller
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

        // quiz attempt controller
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


        //attempt controller
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

        // quiz attempt controller
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

        public IActionResult Create()
        {
            var quiz = new QuizDto();
            return View(quiz);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(QuizDto quizDto)
        {
            if (!ModelState.IsValid)
                return View(quizDto);

            var quiz = _mapper.Map<Quiz>(quizDto);

            for (int i = 0; i < quiz.Questions.Count; i++)
            {
                var question = quiz.Questions[i];
                question.Quiz = quiz;
            }

            await _quizRepository.AddQuizAsync(quiz);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var quiz = await _quizRepository.GetQuizWithQuestionsAndAnswersAsync(id);
            if (quiz == null) return NotFound();

            var quizDto = _mapper.Map<QuizDto>(quiz);

            return View(quizDto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(QuizDto quizDto)
        {
            var quiz = _mapper.Map<Quiz>(quizDto);

            if (ModelState.IsValid)
            {
                await _quizRepository.UpdateQuizAsync(quiz);
                return RedirectToAction(nameof(Index));
            }
            return View(quizDto);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var quiz = await _quizRepository.GetQuizByIdAsync(id);
            if (quiz == null) return NotFound();

            return View(quiz);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _quizRepository.DeleteQuizAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
