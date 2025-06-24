using QuizApp.Application.Common.Interfaces;

namespace QuizApp.Application.Quizzes.Commands;

public class DeleteQuizCommand : ICommand
{
    public Guid Id { get; set; }

    public DeleteQuizCommand(Guid id)
    {
        Id = id;
    }
}
