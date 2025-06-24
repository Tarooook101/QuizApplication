using QuizApp.Application.Common.Helpers;
using QuizApp.Application.Common.Interfaces;
using QuizApp.Application.Common.Models;
using QuizApp.Application.QuizReviews.Commands;
using QuizApp.Domain.Repositories;


namespace QuizApp.Application.QuizReviews.Handlers;

public class UpdateQuizReviewCommandHandler : BaseHandler, ICommandHandler<UpdateQuizReviewCommand>
{
    private readonly IQuizReviewRepository _quizReviewRepository;
    private readonly ICurrentUserService _currentUserService;

    public UpdateQuizReviewCommandHandler(
        IUnitOfWork unitOfWork,
        IQuizReviewRepository quizReviewRepository,
        ICurrentUserService currentUserService) : base(unitOfWork)
    {
        _quizReviewRepository = quizReviewRepository;
        _currentUserService = currentUserService;
    }

    public async Task<Result> Handle(UpdateQuizReviewCommand request, CancellationToken cancellationToken)
    {
        if (!_currentUserService.IsAuthenticated || string.IsNullOrEmpty(_currentUserService.UserId))
            return Result.Failure("User must be authenticated to update a review");

        var userId = Guid.Parse(_currentUserService.UserId);

        var review = await _quizReviewRepository.GetByIdAsync(request.Id, cancellationToken);
        if (review == null)
            return Result.Failure("Review not found");

        if (review.UserId != userId)
            return Result.Failure("User can only update their own reviews");

        review.Update(
            request.Rating,
        request.Comment,
        request.IsRecommended,
            request.IsPublic,
            _currentUserService.UserId);

        await _quizReviewRepository.UpdateAsync(review, cancellationToken);
        await UnitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
