using QuizApp.Application.Common.Interfaces;

namespace QuizApp.Application.QuizResults.Commands;

public class CreateQuizResultCommand : ICommand<Guid>
{
    public Guid QuizAttemptId { get; set; }
    public Guid UserId { get; set; }
    public Guid QuizId { get; set; }
    public int Score { get; set; }
    public int MaxScore { get; set; }
    public int CorrectAnswers { get; set; }
    public int TotalQuestions { get; set; }
    public TimeSpan TimeSpent { get; set; }
    public double? PassingThreshold { get; set; }
    public string? Feedback { get; set; }
}