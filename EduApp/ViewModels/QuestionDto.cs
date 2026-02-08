using System.ComponentModel.DataAnnotations;

namespace EduApp.ViewModels
{
    public class QuestionDto
    {
        public int QuizId { get; set; }
        
        [Required]
        public string Text { get; set; }

        public List<AnswerDto> Answers { get; set; } = new();

        [Required(ErrorMessage = "Select answer")]
        public int SelectedAnswerIndex { get; set; }

        public int QuestionIndex { get; set; }

        public int TotalQuestions { get; set; }
        public int AttemptId { get; set; }
    }
}
