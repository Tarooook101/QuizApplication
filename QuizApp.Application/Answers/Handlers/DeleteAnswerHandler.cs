using QuizApp.Application.Answers.Commands;
using QuizApp.Application.Common.Helpers;
using QuizApp.Application.Common.Interfaces;
using QuizApp.Application.Common.Models;
using QuizApp.Domain.Repositories;


namespace QuizApp.Application.Answers.Handlers;

public class DeleteAnswerHandler : BaseHandler, ICommandHandler<DeleteAnswerCommand>
{
    private readonly IAnswerRepository _answerRepository;

    public DeleteAnswerHandler(
        IUnitOfWork unitOfWork,
        IAnswerRepository answerRepository) : base(unitOfWork)
    {
        _answerRepository = answerRepository;
    }

    public async Task<Result> Handle(DeleteAnswerCommand request, CancellationToken cancellationToken)
    {
        var answer = await _answerRepository.GetByIdAsync(request.Id, cancellationToken);
        if (answer == null)
            return Result.Failure("Answer not found");

        await _answerRepository.DeleteAsync(answer, cancellationToken);
        await UnitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}