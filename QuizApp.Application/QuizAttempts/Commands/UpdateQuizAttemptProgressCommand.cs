
using QuizApp.Application.Common.Interfaces;

namespace QuizApp.Application.QuizAttempts.Commands;

public class UpdateQuizAttemptProgressCommand : ICommand
{
    public Guid Id { get; set; }
    public string? Notes { get; set; }
}