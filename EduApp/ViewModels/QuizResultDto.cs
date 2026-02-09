using EduApp.Models;

namespace EduApp.ViewModels
{
    public class QuizResultDto
    {
        public int Score { get; set; }
        public int Total { get; set; }

        public int Percentage =>
            Total == 0 ? 0 : (int)((double)Score / Total * 100);

        public string Message =>
            Percentage switch
            {
                >= 90 => "Excellent! 🚀",
                >= 70 => "Great job! 👏",
                >= 50 => "Not bad 🙂",
                _ => "Keep practicing 💪"
            };

        public int QuizId { get; internal set; }
    }
}
