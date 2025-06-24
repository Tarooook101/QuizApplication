

using QuizApp.Application.Common.Interfaces;

namespace QuizApp.Application.Quizzes.Commands;

public class DeactivateQuizCommand : ICommand
{
    public Guid Id { get; set; }

    public DeactivateQuizCommand(Guid id)
    {
        Id = id;
    }
}