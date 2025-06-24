

using QuizApp.Application.Common.Interfaces;

namespace QuizApp.Application.UserAnswers.Commands;

public class DeleteUserAnswerCommand : ICommand
{
    public Guid Id { get; set; }
}