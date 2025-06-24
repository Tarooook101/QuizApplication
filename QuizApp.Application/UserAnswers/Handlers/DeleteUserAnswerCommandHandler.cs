using QuizApp.Application.Common.Helpers;
using QuizApp.Application.Common.Interfaces;
using QuizApp.Application.Common.Models;
using QuizApp.Application.UserAnswers.Commands;
using QuizApp.Domain.Events.UserAnswerEvents;
using QuizApp.Domain.Repositories;


namespace QuizApp.Application.UserAnswers.Handlers;

public class DeleteUserAnswerCommandHandler : BaseHandler, ICommandHandler<DeleteUserAnswerCommand>
{
    private readonly IUserAnswerRepository _userAnswerRepository;

    public DeleteUserAnswerCommandHandler(
        IUnitOfWork unitOfWork,
        IUserAnswerRepository userAnswerRepository) : base(unitOfWork)
    {
        _userAnswerRepository = userAnswerRepository;
    }

    public async Task<Result> Handle(DeleteUserAnswerCommand request, CancellationToken cancellationToken)
    {
        var userAnswer = await _userAnswerRepository.GetByIdAsync(request.Id, cancellationToken);
        if (userAnswer == null)
            return Result.Failure("User answer not found");

        userAnswer.AddDomainEvent(new UserAnswerDeletedEvent(userAnswer.Id, userAnswer.QuizAttemptId, userAnswer.QuestionId));

        await _userAnswerRepository.DeleteAsync(userAnswer, cancellationToken);
        await UnitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}