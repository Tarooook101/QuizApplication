using QuizApp.Application.Common.Helpers;
using QuizApp.Application.Common.Interfaces;
using QuizApp.Application.Common.Models;
using QuizApp.Application.QuizReviews.Commands;
using QuizApp.Domain.Entities;
using QuizApp.Domain.Repositories;

namespace QuizApp.Application.QuizReviewss.Handlers;

public class CreateQuizReviewCommandHandler : BaseHandler, ICommandHandler<CreateQuizReviewCommand, Guid>
{
    private readonly IQuizReviewRepository _quizReviewRepository;
    private readonly IRepository<Quiz, Guid> _quizRepository;
    private readonly ICurrentUserService _currentUserService;

    public CreateQuizReviewCommandHandler(
        IUnitOfWork unitOfWork,
        IQuizReviewRepository quizReviewRepository,
        IRepository<Quiz, Guid> quizRepository,
        ICurrentUserService currentUserService) : base(unitOfWork)
    {
        _quizReviewRepository = quizReviewRepository;
        _quizRepository = quizRepository;
        _currentUserService = currentUserService;
    }

    public async Task<Result<Guid>> Handle(CreateQuizReviewCommand request, CancellationToken cancellationToken)
    {
        if (!_currentUserService.IsAuthenticated || string.IsNullOrEmpty(_currentUserService.UserId))
            return Result.Failure<Guid>("User must be authenticated to create a review");

        var userId = Guid.Parse(_currentUserService.UserId);

        var quiz = await _quizRepository.GetByIdAsync(request.QuizId, cancellationToken);
        if (quiz == null)
            return Result.Failure<Guid>("Quiz not found");

        if (!quiz.IsActive)
            return Result.Failure<Guid>("Cannot review an inactive quiz");

        var existingReview = await _quizReviewRepository.GetUserReviewForQuizAsync(userId, request.QuizId, cancellationToken);
        if (existingReview != null) 
            return Result.Failure<Guid>("User has already reviewed this quiz");

        var review = new QuizApp.Domain.Entities.QuizReview(
            request.QuizId,
            userId,
            request.Rating,
            request.Comment,
            request.IsRecommended,
            request.IsPublic);

        await _quizReviewRepository.AddAsync(review, cancellationToken);
        await UnitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(review.Id);
    }
}