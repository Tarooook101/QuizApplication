using QuizApp.Application.Common.Helpers;
using QuizApp.Application.Common.Interfaces;
using QuizApp.Application.Common.Models;
using QuizApp.Application.Quizzes.Commands;
using QuizApp.Domain.Entities;
using QuizApp.Domain.Repositories;


namespace QuizApp.Application.Quizzes.Handlers;

public class CreateQuizCommandHandler : BaseHandler, ICommandHandler<CreateQuizCommand, Guid>
{
    private readonly IQuizRepository _quizRepository;
    private readonly ICurrentUserService _currentUserService;

    public CreateQuizCommandHandler(
        IUnitOfWork unitOfWork,
        IQuizRepository quizRepository,
        ICurrentUserService currentUserService) : base(unitOfWork)
    {
        _quizRepository = quizRepository;
        _currentUserService = currentUserService;
    }

    public async Task<Result<Guid>> Handle(CreateQuizCommand request, CancellationToken cancellationToken)
    {
        if (!_currentUserService.IsAuthenticated || string.IsNullOrEmpty(_currentUserService.UserId))
        {
            return Result.Failure<Guid>("User must be authenticated to create a quiz");
        }

        if (!Guid.TryParse(_currentUserService.UserId, out var userId))
        {
            return Result.Failure<Guid>("Invalid user ID");
        }

        var quiz = new Quiz(
            request.Title,
            request.Description,
            request.TimeLimit,
            request.MaxAttempts,
        request.Difficulty,
        userId,
            request.Instructions,
            request.IsPublic,
            request.ThumbnailUrl,
            request.Tags);

        await _quizRepository.AddAsync(quiz, cancellationToken);
        await UnitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(quiz.Id);
    }
}
