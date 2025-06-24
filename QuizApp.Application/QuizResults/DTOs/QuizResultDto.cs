using QuizApp.Domain.Enums;


namespace QuizApp.Application.QuizResults.DTOs;

public class QuizResultDto
{
    public Guid Id { get; set; }
    public Guid QuizAttemptId { get; set; }
    public Guid UserId { get; set; }
    public Guid QuizId { get; set; }
    public int Score { get; set; }
    public int MaxScore { get; set; }
    public double Percentage { get; set; }
    public int CorrectAnswers { get; set; }
    public int TotalQuestions { get; set; }
    public TimeSpan TimeSpent { get; set; }
    public QuizResultStatus Status { get; set; }
    public string? Feedback { get; set; }
    public DateTime CompletedAt { get; set; }
    public bool IsPassed { get; set; }
    public double? PassingThreshold { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}