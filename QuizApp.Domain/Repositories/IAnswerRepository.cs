using QuizApp.Domain.Entities;


namespace QuizApp.Domain.Repositories;

public interface IAnswerRepository : IRepository<Answer, Guid>
{
    Task<IEnumerable<Answer>> GetByQuestionIdAsync(Guid questionId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Answer>> GetByQuestionIdOrderedAsync(Guid questionId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Answer>> GetCorrectAnswersByQuestionIdAsync(Guid questionId, CancellationToken cancellationToken = default);
    Task<Answer?> GetByQuestionIdAndOrderAsync(Guid questionId, int orderIndex, CancellationToken cancellationToken = default);
    Task<int> GetMaxOrderIndexByQuestionIdAsync(Guid questionId, CancellationToken cancellationToken = default);
    Task<bool> ExistsInQuestionAsync(Guid answerId, Guid questionId, CancellationToken cancellationToken = default);
    Task<int> CountByQuestionIdAsync(Guid questionId, CancellationToken cancellationToken = default);
    Task<int> CountCorrectAnswersByQuestionIdAsync(Guid questionId, CancellationToken cancellationToken = default);
}