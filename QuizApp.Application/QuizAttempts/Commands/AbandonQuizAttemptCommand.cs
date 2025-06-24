using QuizApp.Application.Common.Interfaces;

namespace QuizApp.Application.QuizAttempts.Commands;

public class AbandonQuizAttemptCommand : ICommand
{
    public Guid Id { get; set; }
}
