namespace QuizApp.Domain
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
        public ICollection<QuestionAttempt> QuestionAttempts { get; set; }

        public int CalculateScore()
        {
            int score = 0;
            foreach (var question in Quiz.Questions)
            {
                var userAnswer = QuestionAttempts.FirstOrDefault(qa => qa.QuestionId == question.Id);
                if (userAnswer != null)
                {
                    var selected = question.Answers.First(a => a.Id == userAnswer.SelectedAnswerId);
                    if (selected.IsCorrect)
                        score++;
                }
            }
            return score;
        }
    }


}
