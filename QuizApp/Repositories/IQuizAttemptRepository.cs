using QuizApp.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizApp.Repositories
{
    public interface IQuizAttemptRepository
    {
        Task AddQuizAttemptAsync(QuizAttempt quizAttempt);
        Task<QuizAttempt?> GetAttemptAsync(int attemptId);
        Task AddQuestionAttemptAsync(QuestionAttempt qa);
        Task UpdateQuizAttemptAsync(QuizAttempt attempt);
        Task<IEnumerable<QuestionAttempt>> GetQuestionAttemptsAsync(int attemptId);
    }
}
