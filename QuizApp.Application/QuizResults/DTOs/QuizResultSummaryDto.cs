using QuizApp.Domain.Enums;


namespace QuizApp.Application.QuizResults.DTOs;

public class QuizResultSummaryDto
{
    public Guid Id { get; set; }
    public int Score { get; set; }
    public int MaxScore { get; set; }
    public double Percentage { get; set; }
    public QuizResultStatus Status { get; set; }
    public bool IsPassed { get; set; }
    public DateTime CompletedAt { get; set; }
}