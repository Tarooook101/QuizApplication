

using QuizApp.Application.Common.Interfaces;

namespace QuizApp.Application.QuizReviews.Commands;

public class DeleteQuizReviewCommand : ICommand
{
    public Guid Id { get; set; }
}

