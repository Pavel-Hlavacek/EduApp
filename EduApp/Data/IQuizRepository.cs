using EduApp.Models;
using Microsoft.EntityFrameworkCore;
using QuizApp.Domain;

namespace EduApp.Data
{
    public interface IQuizRepository
    {
        IEnumerable<Quiz> GetAllQuizzes();
        Quiz GetQuizById(int id);
        void AddQuiz(Quiz quiz);
        void UpdateQuiz(Quiz quiz);
        void DeleteQuiz(int id);
        Quiz GetQuizWithQuestionsAndAnswers(int quizId);

        void AddQuizAttempt(QuizAttempt quizAttempt);
        QuizAttempt GetAttempt(int attemptId);
        void AddQuestionAttempt(QuestionAttempt qa);
        void UpdateQuizAttempt(QuizAttempt attempt);
        IEnumerable<QuestionAttempt> GetQuestionAttempts(int attemptId);
    }

    public class QuizRepository : IQuizRepository
    {
        private readonly ApplicationDbContext _context;
        public QuizRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public void AddQuestionAttempt(QuestionAttempt qa)
        {
            _context.QuestionAttempts.Add(qa);
            _context.SaveChanges();
        }

        public void AddQuiz(Quiz quiz)
        {
            if (quiz is null) return;
            _context.Quizzes.Add(quiz);
            _context.SaveChanges();
        }

        public void AddQuizAttempt(QuizAttempt quizAttempt)
        {
            _context.QuizAttempts.Add(quizAttempt);
            _context.SaveChanges();
        }

        public void DeleteQuiz(int id)
        {
            var quiz = _context.Quizzes.Find(id);
            _context.Quizzes.Remove(quiz);
            _context.SaveChanges();
        }

        public IEnumerable<Quiz> GetAllQuizzes()
        {
            return _context.Quizzes;
        }

        public QuizAttempt GetAttempt(int attemptId)
        {
            return _context.QuizAttempts
                           .Include(a => a.Quiz)
                           .ThenInclude(q => q.Questions)
                           .ThenInclude(q => q.Answers)
                           .First(a => a.Id == attemptId);
        }

        public IEnumerable<QuestionAttempt> GetQuestionAttempts(int attemptId)
        {
            return _context.QuestionAttempts.Where(a => a.QuizAttemptId == attemptId); 
        }

        public Quiz GetQuizById(int id)
        {
            return _context.Quizzes.Find(id);
        }

        public Quiz GetQuizWithQuestionsAndAnswers(int quizId)
        {
            return _context.Quizzes
                        .Include(q => q.Questions)
                        .ThenInclude(qn => qn.Answers)
                        .FirstOrDefault(q => q.Id == quizId);
        }

        public void UpdateQuiz(Quiz quiz)
        {
            _context.Quizzes.Update(quiz);
            _context.SaveChanges();
        }

        public void UpdateQuizAttempt(QuizAttempt attempt)
        {
            _context.QuizAttempts.Update(attempt);
        }
    }

}
