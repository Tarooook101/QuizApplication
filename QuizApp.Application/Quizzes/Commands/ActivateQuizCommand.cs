

using QuizApp.Application.Common.Interfaces;

namespace QuizApp.Application.Quizzes.Commands;

public class ActivateQuizCommand : ICommand
{
    public Guid Id { get; set; }

    public ActivateQuizCommand(Guid id)
    {
        Id = id;
    }
}
