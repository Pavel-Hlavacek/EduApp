using Microsoft.EntityFrameworkCore;
using QuizApp.Domain;
using QuizApp.Repositories;

namespace EduApp.Data
{
    public class QuizRepository : IQuizRepository
    {
        private readonly ApplicationDbContext _context;
        public QuizRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddQuestionAttemptAsync(QuestionAttempt qa)
        {
            _context.QuestionAttempts.Add(qa);
            await _context.SaveChangesAsync();
        }

        public async Task AddQuizAsync(Quiz quiz)
        {
            if (quiz is null) return;
            _context.Quizzes.Add(quiz);
            await _context.SaveChangesAsync();
        }

        public async Task AddQuizAttemptAsync(QuizAttempt quizAttempt)
        {
            _context.QuizAttempts.Add(quizAttempt);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteQuizAsync(int id)
        {
            var quiz = await _context.Quizzes.FindAsync(id);
            if (quiz is null) return;

            _context.Quizzes.Remove(quiz);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Quiz>> GetAllQuizzesAsync()
        {
            return await _context.Quizzes.AsNoTracking().ToListAsync();
        }

        public async Task<QuizAttempt?> GetAttemptAsync(int attemptId)
        {
            return await _context.QuizAttempts
                           .Include(a => a.Quiz)
                           .ThenInclude(q => q.Questions)
                           .ThenInclude(q => q.Answers)
                           .FirstOrDefaultAsync(a => a.Id == attemptId);
        }

        public async Task<IEnumerable<QuestionAttempt>> GetQuestionAttemptsAsync(int attemptId)
        {
            return await _context.QuestionAttempts.Where(a => a.QuizAttemptId == attemptId).ToListAsync(); 
        }

        public async Task<Quiz?> GetQuizByIdAsync(int id)
        {
            return await _context.Quizzes.FindAsync(id);
        }

        public async Task<Quiz?> GetQuizWithQuestionsAndAnswersAsync(int quizId)
        {
            return await _context.Quizzes
                        .Include(q => q.Questions)
                        .ThenInclude(qn => qn.Answers)
                        .FirstOrDefaultAsync(q => q.Id == quizId);
        }

        public async Task UpdateQuizAsync(Quiz quiz)
        {
            _context.Quizzes.Update(quiz);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateQuizAttemptAsync(QuizAttempt attempt)
        {
            _context.QuizAttempts.Update(attempt);
            await _context.SaveChangesAsync();
        }
    }

}
