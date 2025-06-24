using QuizApp.Application.Common.Helpers;
using QuizApp.Application.Common.Interfaces;
using QuizApp.Application.Common.Models;
using QuizApp.Application.QuizReviews.Commands;
using QuizApp.Domain.Repositories;


namespace QuizApp.Application.QuizReviews.Handlers;

public class DeleteQuizReviewCommandHandler : BaseHandler, ICommandHandler<DeleteQuizReviewCommand>
{
    private readonly IQuizReviewRepository _quizReviewRepository;
    private readonly ICurrentUserService _currentUserService;

    public DeleteQuizReviewCommandHandler(
        IUnitOfWork unitOfWork,
        IQuizReviewRepository quizReviewRepository,
        ICurrentUserService currentUserService) : base(unitOfWork)
    {
        _quizReviewRepository = quizReviewRepository;
        _currentUserService = currentUserService;
    }

    public async Task<Result> Handle(DeleteQuizReviewCommand request, CancellationToken cancellationToken)
    {
        if (!_currentUserService.IsAuthenticated || string.IsNullOrEmpty(_currentUserService.UserId))
            return Result.Failure("User must be authenticated to delete a review");

        var userId = Guid.Parse(_currentUserService.UserId);

        var review = await _quizReviewRepository.GetByIdAsync(request.Id, cancellationToken);
        if (review == null)
            return Result.Failure("Review not found");

        if (review.UserId != userId)
            return Result.Failure("User can only delete their own reviews");

        await _quizReviewRepository.DeleteAsync(review, cancellationToken);
        await UnitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}