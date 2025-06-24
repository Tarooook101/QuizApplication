using QuizApp.Domain.Entities;


namespace QuizApp.Domain.Repositories;

public interface IQuizResultRepository : IRepository<QuizResult, Guid>
{
    Task<IEnumerable<QuizResult>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<IEnumerable<QuizResult>> GetByQuizIdAsync(Guid quizId, CancellationToken cancellationToken = default);
    Task<IEnumerable<QuizResult>> GetPassedResultsAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<QuizResult>> GetFailedResultsAsync(CancellationToken cancellationToken = default);
    Task<QuizResult?> GetByQuizAttemptIdAsync(Guid quizAttemptId, CancellationToken cancellationToken = default);
    Task<IEnumerable<QuizResult>> GetTopScoresAsync(Guid quizId, int count = 10, CancellationToken cancellationToken = default);
    Task<double> GetAverageScoreAsync(Guid quizId, CancellationToken cancellationToken = default);
}