using QuizApp.Domain.Enums;


namespace QuizApp.Application.QuizAttempts.DTOs;

public class QuizAttemptDto
{
    public Guid Id { get; set; }
    public Guid QuizId { get; set; }
    public Guid UserId { get; set; }
    public DateTime StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public QuizAttemptStatus Status { get; set; }
    public int? Score { get; set; }
    public int? MaxScore { get; set; }
    public double? Percentage { get; set; }
    public int TimeSpentMinutes { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}