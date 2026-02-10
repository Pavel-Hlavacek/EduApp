namespace QuizApp.Domain
{
    public class QuestionAttempt
    {
        public int Id { get; set; }
        public int QuizAttemptId { get; set; }
        public QuizAttempt QuizAttempt { get; set; }
        public int QuestionId { get; set; }
        public int SelectedAnswerId { get; set; }
    }


}
