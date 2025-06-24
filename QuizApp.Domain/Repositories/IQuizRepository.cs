using QuizApp.Domain.Entities;
using QuizApp.Domain.Enums;
using QuizApp.Domain.Specifications;


namespace QuizApp.Domain.Repositories;

public interface IQuizRepository : IRepository<Quiz, Guid>
{
    Task<IEnumerable<Quiz>> GetActiveQuizzesAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<Quiz>> GetPublicQuizzesAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<Quiz>> GetQuizzesByUserAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Quiz>> GetQuizzesByDifficultyAsync(QuizDifficulty difficulty, CancellationToken cancellationToken = default);
    Task<bool> ExistsByTitleAsync(string title, Guid? excludeId = null, CancellationToken cancellationToken = default);
}