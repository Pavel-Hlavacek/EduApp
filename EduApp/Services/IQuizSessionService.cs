using System.Text.Json;

namespace EduApp.Services
{
    public interface IQuizSessionService
    {
        Dictionary<int, int> GetAnswers();
        void SaveAnswer(int questionIndex, int selectedAnswer);
        void ClearAnswers();
    }


    public class QuizSessionService : IQuizSessionService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private const string QuizSessionKey = "QUIZ_ANSWERS";

        public QuizSessionService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        private ISession Session => _httpContextAccessor.HttpContext!.Session;

        public Dictionary<int, int> GetAnswers()
        {
            var data = Session.GetString(QuizSessionKey);
            if (string.IsNullOrEmpty(data))
                return new Dictionary<int, int>();

            return JsonSerializer.Deserialize<Dictionary<int, int>>(data)!;
        }

        public void SaveAnswer(int questionIndex, int selectedAnswer)
        {
            var answers = GetAnswers();
            answers[questionIndex] = selectedAnswer;
            Session.SetString(QuizSessionKey, JsonSerializer.Serialize(answers));
        }

        public void ClearAnswers()
        {
            Session.Remove(QuizSessionKey);
        }
    }
}
