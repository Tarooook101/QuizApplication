using QuizApp.Domain.Entities;


namespace QuizApp.Domain.Repositories;

public interface IQuizAttemptRepository : IRepository<QuizAttempt, Guid>
{
    Task<IEnumerable<QuizAttempt>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<IEnumerable<QuizAttempt>> GetByQuizIdAsync(Guid quizId, CancellationToken cancellationToken = default);
    Task<IEnumerable<QuizAttempt>> GetCompletedAttemptsAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<QuizAttempt>> GetInProgressAttemptsAsync(CancellationToken cancellationToken = default);
    Task<int> GetAttemptCountForUserAndQuizAsync(Guid userId, Guid quizId, CancellationToken cancellationToken = default);
    Task<QuizAttempt?> GetActiveAttemptAsync(Guid userId, Guid quizId, CancellationToken cancellationToken = default);
}