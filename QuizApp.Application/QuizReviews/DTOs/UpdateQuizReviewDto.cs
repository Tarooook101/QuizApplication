

namespace QuizApp.Application.QuizReviews.DTOs;

public class UpdateQuizReviewDto
{
    public int Rating { get; set; }
    public string? Comment { get; set; }
    public bool IsRecommended { get; set; }
    public bool IsPublic { get; set; }
}