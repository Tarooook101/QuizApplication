using QuizApp.Application.Common.Helpers;
using QuizApp.Application.Common.Interfaces;
using QuizApp.Application.Common.Models;
using QuizApp.Application.Quizzes.Commands;
using QuizApp.Domain.Repositories;


namespace QuizApp.Application.Quizzes.Handlers;

public class DeleteQuizCommandHandler : BaseHandler, ICommandHandler<DeleteQuizCommand>
{
    private readonly IQuizRepository _quizRepository;
    private readonly ICurrentUserService _currentUserService;

    public DeleteQuizCommandHandler(
        IUnitOfWork unitOfWork,
        IQuizRepository quizRepository,
        ICurrentUserService currentUserService) : base(unitOfWork)
    {
        _quizRepository = quizRepository;
        _currentUserService = currentUserService;
    }

    public async Task<Result> Handle(DeleteQuizCommand request, CancellationToken cancellationToken)
    {
        var quiz = await _quizRepository.GetByIdAsync(request.Id, cancellationToken);
        if (quiz == null)
        {
            return Result.Failure("Quiz not found");
        }

        if (!_currentUserService.IsAuthenticated || string.IsNullOrEmpty(_currentUserService.UserId))
        {
            return Result.Failure("User must be authenticated to delete a quiz");
        }

        if (!Guid.TryParse(_currentUserService.UserId, out var userId) || quiz.CreatedByUserId != userId)
        {
            return Result.Failure("You can only delete your own quizzes");
        }

        await _quizRepository.DeleteAsync(quiz, cancellationToken);
        await UnitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}