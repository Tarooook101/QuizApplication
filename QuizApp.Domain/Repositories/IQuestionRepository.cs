using QuizApp.Domain.Entities;


namespace QuizApp.Domain.Repositories;
public interface IQuestionRepository : IRepository<Question, Guid>
{
    Task<IEnumerable<Question>> GetByQuizIdAsync(Guid quizId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Question>> GetByQuizIdOrderedAsync(Guid quizId, CancellationToken cancellationToken = default);
    Task<Question?> GetByQuizIdAndOrderAsync(Guid quizId, int orderIndex, CancellationToken cancellationToken = default);
    Task<int> GetMaxOrderIndexByQuizIdAsync(Guid quizId, CancellationToken cancellationToken = default);
    Task<bool> ExistsInQuizAsync(Guid questionId, Guid quizId, CancellationToken cancellationToken = default);
    Task<int> CountByQuizIdAsync(Guid quizId, CancellationToken cancellationToken = default);
}