using QuizApp.Application.Common.Interfaces;

namespace QuizApp.Application.Answers.Commands;

public class DeleteAnswerCommand : ICommand
{
    public Guid Id { get; set; }

    public DeleteAnswerCommand(Guid id)
    {
        Id = id;
    }
}