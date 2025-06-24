namespace QuizApp.Application.QuizReviews.DTOs;

public class QuizReviewSummaryDto
{
    public Guid QuizId { get; set; }
    public double AverageRating { get; set; }
    public int TotalReviews { get; set; }
    public int FiveStarCount { get; set; }
    public int FourStarCount { get; set; }
    public int ThreeStarCount { get; set; }
    public int TwoStarCount { get; set; }
    public int OneStarCount { get; set; }
    public int RecommendedCount { get; set; }
    public double RecommendationPercentage { get; set; }
}