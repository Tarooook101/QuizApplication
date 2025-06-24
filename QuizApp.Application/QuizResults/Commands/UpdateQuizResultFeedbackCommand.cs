

using QuizApp.Application.Common.Interfaces;

namespace QuizApp.Application.QuizResults.Commands;

public class UpdateQuizResultFeedbackCommand : ICommand
{
    public Guid Id { get; set; }
    public string Feedback { get; set; } = string.Empty;
}