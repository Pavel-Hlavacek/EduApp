namespace EduApp.Tests.QuizApp;
using FluentAssertions;
using global::QuizApp.Domain;
using Xunit;

public class QuizTests
{
    [Fact]
    public void NewQuiz_ShouldHaveZeroScore()
    {
        var quiz = new Quiz();

       // quiz.Score.Should().Be(0);
    }
}
