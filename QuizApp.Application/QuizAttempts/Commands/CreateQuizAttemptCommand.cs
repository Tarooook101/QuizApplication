using QuizApp.Application.Common.Interfaces;

namespace QuizApp.Application.QuizAttempts.Commands;

public class CreateQuizAttemptCommand : ICommand<Guid>
{
    public Guid QuizId { get; set; }
}