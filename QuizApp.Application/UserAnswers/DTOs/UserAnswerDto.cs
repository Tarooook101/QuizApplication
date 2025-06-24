namespace QuizApp.Application.UserAnswers.DTOs;

public class UserAnswerDto
{
    public Guid Id { get; set; }
    public Guid QuizAttemptId { get; set; }
    public Guid QuestionId { get; set; }
    public Guid? SelectedAnswerId { get; set; }
    public string? TextAnswer { get; set; }
    public bool IsCorrect { get; set; }
    public int PointsEarned { get; set; }
    public DateTime AnsweredAt { get; set; }
    public TimeSpan TimeSpent { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public string? UpdatedBy { get; set; }
}