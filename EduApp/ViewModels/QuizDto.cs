using EduApp.Models;
using QuizApp.Domain;
using System.ComponentModel.DataAnnotations;

namespace EduApp.ViewModels
{
    public class QuizDto
    {
        [Required]
        public string Title { get; set; }
        public string Description { get; set; }
        public List<QuestionDto> Questions { get; set; } = new();
        public int Id { get; init; }
    }
}
