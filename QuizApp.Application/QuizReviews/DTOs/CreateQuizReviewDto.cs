namespace QuizApp.Application.QuizReviews.DTOs;

public class CreateQuizReviewDto
{
    public Guid QuizId { get; set; }
    public int Rating { get; set; }
    public string? Comment { get; set; }
    public bool IsRecommended { get; set; } = true;
    public bool IsPublic { get; set; } = true;
}
