using EduApp.Data;
using Microsoft.AspNetCore.Identity;
using QuizApp.Domain;
using System.ComponentModel.DataAnnotations.Schema;

namespace EduApp.Models
{
    public class QuizAttempt
    {
        public int Id { get; set; }
        public int QuizId { get; set; }
        public Quiz Quiz { get; set; }
        public string UserId { get; set; }
        public DateTime StartedAt { get; set; }
        public DateTime? FinishedAt { get; set; }
        public int Score { get; set; }
        //public List<UserAnswer> Answers { get; set; } = new();
        public ICollection<QuestionAttempt> QuestionAttempts { get; set; }
    }

    public class QuestionAttempt
    {
        public int Id { get; set; }
        public int QuizAttemptId { get; set; }
        public QuizAttempt QuizAttempt { get; set; }
        public int QuestionId { get; set; }
        public int SelectedAnswerId { get; set; }
    }


}
