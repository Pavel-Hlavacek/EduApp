using AutoMapper;
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
        private readonly IMapper _mapper;
        private readonly ILogger<QuizController> _logger;

        public QuizController(
            IQuizRepository quizRepository,
            IMapper mapper,
            ILogger<QuizController> logger)
        {
            _quizRepository = quizRepository;
            _mapper = mapper;
            _logger = logger;
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
