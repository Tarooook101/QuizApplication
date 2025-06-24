using QuizApp.Application.Common.Interfaces;


namespace QuizApp.Application.Questions.Commands;

public class DeleteQuestionCommand : ICommand
{
    public Guid Id { get; set; }

    public DeleteQuestionCommand(Guid id)
    {
        Id = id;
    }
}
