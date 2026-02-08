using System.ComponentModel.DataAnnotations;

namespace EduApp.ViewModels
{
    public class AnswerDto
    {
        [Required]
        public string Text { get; set; }
        public bool IsCorrect { get; set; }

        public override string ToString()
        {
            return Text;
        }
    }
}
