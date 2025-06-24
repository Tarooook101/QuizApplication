using QuizApp.Application.Common.Helpers;
using QuizApp.Application.Common.Interfaces;
using QuizApp.Application.Common.Models;
using QuizApp.Application.Quizzes.Commands;
using QuizApp.Domain.Repositories;


namespace QuizApp.Application.Quizzes.Handlers;

public class UpdateQuizCommandHandler : BaseHandler, ICommandHandler<UpdateQuizCommand>
{
    private readonly IQuizRepository _quizRepository;
    private readonly ICurrentUserService _currentUserService;

    public UpdateQuizCommandHandler(
        IUnitOfWork unitOfWork,
        IQuizRepository quizRepository,
        ICurrentUserService currentUserService) : base(unitOfWork)
    {
        _quizRepository = quizRepository;
        _currentUserService = currentUserService;
    }

    public async Task<Result> Handle(UpdateQuizCommand request, CancellationToken cancellationToken)
    {
        var quiz = await _quizRepository.GetByIdAsync(request.Id, cancellationToken);
        if (quiz == null)
        {
            return Result.Failure("Quiz not found");
        }

        if (!_currentUserService.IsAuthenticated || string.IsNullOrEmpty(_currentUserService.UserId))
        {
            return Result.Failure("User must be authenticated to update a quiz");
        }

        if (!Guid.TryParse(_currentUserService.UserId, out var userId) || quiz.CreatedByUserId != userId)
        {
            return Result.Failure("You can only update your own quizzes");
        }

        quiz.Update(
            request.Title,
            request.Description,
            request.Instructions,
            request.TimeLimit,
            request.MaxAttempts,
            request.Difficulty,
        request.IsPublic,
            request.ThumbnailUrl,
            request.Tags,
            _currentUserService.UserName);

        await _quizRepository.UpdateAsync(quiz, cancellationToken);
        await UnitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}