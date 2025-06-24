using QuizApp.Application.Common.Helpers;
using QuizApp.Application.Common.Interfaces;
using QuizApp.Application.Common.Models;
using QuizApp.Application.QuizAttempts.Commands;
using QuizApp.Domain.Entities;
using QuizApp.Domain.Repositories;


namespace QuizApp.Application.QuizAttempts.Handlers;

public class CreateQuizAttemptHandler : BaseHandler, ICommandHandler<CreateQuizAttemptCommand, Guid>
{
    private readonly IQuizAttemptRepository _quizAttemptRepository;
    private readonly IQuizRepository _quizRepository;
    private readonly ICurrentUserService _currentUserService;

    public CreateQuizAttemptHandler(
        IUnitOfWork unitOfWork,
        IQuizAttemptRepository quizAttemptRepository,
        IQuizRepository quizRepository,
        ICurrentUserService currentUserService) : base(unitOfWork)
    {
        _quizAttemptRepository = quizAttemptRepository;
        _quizRepository = quizRepository;
        _currentUserService = currentUserService;
    }

    public async Task<Result<Guid>> Handle(CreateQuizAttemptCommand request, CancellationToken cancellationToken)
    {
        if (!Guid.TryParse(_currentUserService.UserId, out var userId))
            return Result.Failure<Guid>("User not authenticated");

        var quiz = await _quizRepository.GetByIdAsync(request.QuizId, cancellationToken);
        if (quiz == null)
            return Result.Failure<Guid>("Quiz not found");

        if (!quiz.IsActive)
            return Result.Failure<Guid>("Quiz is not active");

        var existingActiveAttempt = await _quizAttemptRepository.GetActiveAttemptAsync(userId, request.QuizId, cancellationToken);
        if (existingActiveAttempt != null)
            return Result.Failure<Guid>("An active attempt already exists for this quiz");

        var attemptCount = await _quizAttemptRepository.GetAttemptCountForUserAndQuizAsync(userId, request.QuizId, cancellationToken);
        if (attemptCount >= quiz.MaxAttempts)
            return Result.Failure<Guid>("Maximum attempts exceeded for this quiz");

        var quizAttempt = new QuizAttempt(request.QuizId, userId);

        await _quizAttemptRepository.AddAsync(quizAttempt, cancellationToken);
        await UnitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(quizAttempt.Id);
    }
}