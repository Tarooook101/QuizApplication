using QuizApp.Domain.Entities;

namespace QuizApp.Domain.Repositories;

public interface IQuizReviewRepository : IRepository<QuizReview, Guid>
{
    Task<IEnumerable<QuizReview>> GetReviewsByQuizAsync(Guid quizId, CancellationToken cancellationToken = default);
    Task<IEnumerable<QuizReview>> GetReviewsByUserAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<IEnumerable<QuizReview>> GetPublicReviewsByQuizAsync(Guid quizId, CancellationToken cancellationToken = default);
    Task<QuizReview?> GetUserReviewForQuizAsync(Guid userId, Guid quizId, CancellationToken cancellationToken = default);
    Task<double> GetAverageRatingForQuizAsync(Guid quizId, CancellationToken cancellationToken = default);
    Task<int> GetReviewCountForQuizAsync(Guid quizId, CancellationToken cancellationToken = default);
    Task<bool> HasUserReviewedQuizAsync(Guid userId, Guid quizId, CancellationToken cancellationToken = default);
}