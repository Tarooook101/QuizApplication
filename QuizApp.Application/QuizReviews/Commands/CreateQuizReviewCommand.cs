using QuizApp.Application.Common.Interfaces;

namespace QuizApp.Application.QuizReviews.Commands;

public class CreateQuizReviewCommand : ICommand<Guid>
{
    public Guid QuizId { get; set; }
    public int Rating { get; set; }
    public string? Comment { get; set; }
    public bool IsRecommended { get; set; } = true;
    public bool IsPublic { get; set; } = true;
}