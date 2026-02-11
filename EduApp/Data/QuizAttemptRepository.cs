using Microsoft.EntityFrameworkCore;
using QuizApp.Domain;
using QuizApp.Repositories;

namespace EduApp.Data
{
    public class QuizAttemptRepository : IQuizAttemptRepository
    {
        private readonly ApplicationDbContext _context;
        public QuizAttemptRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddQuestionAttemptAsync(QuestionAttempt qa)
        {
            _context.QuestionAttempts.Add(qa);
            await _context.SaveChangesAsync();
        }

        public async Task AddQuizAttemptAsync(QuizAttempt quizAttempt)
        {
            _context.QuizAttempts.Add(quizAttempt);
            await _context.SaveChangesAsync();
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

        public async Task UpdateQuizAttemptAsync(QuizAttempt attempt)
        {
            _context.QuizAttempts.Update(attempt);
            await _context.SaveChangesAsync();
        }
    }
}
