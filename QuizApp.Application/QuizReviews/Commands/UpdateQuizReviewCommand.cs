using QuizApp.Application.Common.Interfaces;

namespace QuizApp.Application.QuizReviews.Commands;

public class UpdateQuizReviewCommand : ICommand
{
    public Guid Id { get; set; }
    public int Rating { get; set; }
    public string? Comment { get; set; }
    public bool IsRecommended { get; set; }
    public bool IsPublic { get; set; }
}
