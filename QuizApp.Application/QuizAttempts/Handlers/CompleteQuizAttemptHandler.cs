using QuizApp.Application.Common.Helpers;
using QuizApp.Application.Common.Interfaces;
using QuizApp.Application.Common.Models;
using QuizApp.Application.QuizAttempts.Commands;
using QuizApp.Domain.Repositories;

namespace QuizApp.Application.QuizAttempts.Handlers;

public class CompleteQuizAttemptHandler : BaseHandler, ICommandHandler<CompleteQuizAttemptCommand>
{
    private readonly IQuizAttemptRepository _quizAttemptRepository;
    private readonly ICurrentUserService _currentUserService;

    public CompleteQuizAttemptHandler(
        IUnitOfWork unitOfWork,
        IQuizAttemptRepository quizAttemptRepository,
        ICurrentUserService currentUserService) : base(unitOfWork)
    {
        _quizAttemptRepository = quizAttemptRepository;
        _currentUserService = currentUserService;
    }

    public async Task<Result> Handle(CompleteQuizAttemptCommand request, CancellationToken cancellationToken)
    {
        if (!Guid.TryParse(_currentUserService.UserId, out var userId))
            return Result.Failure("User not authenticated");

        var quizAttempt = await _quizAttemptRepository.GetByIdAsync(request.Id, cancellationToken);
        if (quizAttempt == null)
            return Result.Failure("Quiz attempt not found");

        if (quizAttempt.UserId != userId)
            return Result.Failure("You can only complete your own quiz attempts");

        try
        {
            quizAttempt.Complete(request.Score, request.MaxScore, request.Notes);

            await _quizAttemptRepository.UpdateAsync(quizAttempt, cancellationToken);
            await UnitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
        catch (InvalidOperationException ex)
        {
            return Result.Failure(ex.Message);
        }
    }
}