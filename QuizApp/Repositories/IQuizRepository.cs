using QuizApp.Domain;

namespace QuizApp.Repositories
{
    public interface IQuizRepository
    {
        Task<IEnumerable<Quiz>> GetAllQuizzesAsync();
        Task<Quiz?> GetQuizByIdAsync(int id);
        Task AddQuizAsync(Quiz quiz);
        Task UpdateQuizAsync(Quiz quiz);
        Task DeleteQuizAsync(int id);
        Task<Quiz?> GetQuizWithQuestionsAndAnswersAsync(int quizId);

        //Task AddQuizAttemptAsync(QuizAttempt quizAttempt);
        //Task<QuizAttempt?> GetAttemptAsync(int attemptId);
        //Task AddQuestionAttemptAsync(QuestionAttempt qa);
        //Task UpdateQuizAttemptAsync(QuizAttempt attempt);
        //Task<IEnumerable<QuestionAttempt>> GetQuestionAttemptsAsync(int attemptId);
    }

}
