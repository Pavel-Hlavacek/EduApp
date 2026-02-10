using QuizApp.Domain;

namespace QuizApp.Repositories
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

}
