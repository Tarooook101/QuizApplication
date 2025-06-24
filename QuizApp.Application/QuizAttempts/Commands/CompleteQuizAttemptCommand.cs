

using QuizApp.Application.Common.Interfaces;

namespace QuizApp.Application.QuizAttempts.Commands;

public class CompleteQuizAttemptCommand : ICommand
{
    public Guid Id { get; set; }
    public int Score { get; set; }
    public int MaxScore { get; set; }
    public string? Notes { get; set; }
}
