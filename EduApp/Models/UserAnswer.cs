using Microsoft.CodeAnalysis.Options;
using QuizApp.Domain;

namespace EduApp.Models
{
    public class UserAnswer
    {
        public int Id { get; set; }
        public int QuizAttemptId { get; set; }
        public int QuestionId { get; set; }
        public int SelectedAnswerId { get; set; }
        public bool IsCorrect { get; set; }
    }
}