namespace QuizApp.Application.QuizReviews.DTOs;

public class QuizReviewDto
{
    public Guid Id { get; set; }
    public Guid QuizId { get; set; }
    public Guid UserId { get; set; }
    public string? UserName { get; set; }
    public string? UserFullName { get; set; }
    public int Rating { get; set; }
    public string? Comment { get; set; }
    public bool IsRecommended { get; set; }
    public bool IsPublic { get; set; }
    public DateTime ReviewDate { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}