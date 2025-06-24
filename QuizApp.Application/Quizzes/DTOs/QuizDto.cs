namespace QuizApp.Application.Quizzes.DTOs;

public class QuizDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? Instructions { get; set; }
    public int TimeLimit { get; set; }
    public int MaxAttempts { get; set; }
    public bool IsPublic { get; set; }
    public bool IsActive { get; set; }
    public string Difficulty { get; set; } = string.Empty;
    public string? ThumbnailUrl { get; set; }
    public string? Tags { get; set; }
    public Guid CreatedByUserId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public string? UpdatedBy { get; set; }
}
