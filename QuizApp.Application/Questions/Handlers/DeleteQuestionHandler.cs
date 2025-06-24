using QuizApp.Application.Common.Helpers;
using QuizApp.Application.Common.Interfaces;
using QuizApp.Application.Common.Models;
using QuizApp.Application.Questions.Commands;
using QuizApp.Domain.Events.QuestionEvents;
using QuizApp.Domain.Repositories;


namespace QuizApp.Application.Questions.Handlers;

public class DeleteQuestionHandler : BaseHandler, ICommandHandler<DeleteQuestionCommand>
{
    private readonly IQuestionRepository _questionRepository;

    public DeleteQuestionHandler(
        IUnitOfWork unitOfWork,
        IQuestionRepository questionRepository) : base(unitOfWork)
    {
        _questionRepository = questionRepository;
    }

    public async Task<Result> Handle(DeleteQuestionCommand request, CancellationToken cancellationToken)
    {
        var question = await _questionRepository.GetByIdAsync(request.Id, cancellationToken);
        if (question == null)
            return Result.Failure("Question not found");

        question.AddDomainEvent(new QuestionDeletedEvent(question.Id, question.QuizId));

        await _questionRepository.DeleteAsync(question, cancellationToken);
        await UnitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}