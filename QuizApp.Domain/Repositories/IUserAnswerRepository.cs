using QuizApp.Domain.Entities;


namespace QuizApp.Domain.Repositories;

public interface IUserAnswerRepository : IRepository<UserAnswer, Guid>
{
    Task<IEnumerable<UserAnswer>> GetByQuizAttemptIdAsync(Guid quizAttemptId, CancellationToken cancellationToken = default);
    Task<IEnumerable<UserAnswer>> GetByQuestionIdAsync(Guid questionId, CancellationToken cancellationToken = default);
    Task<UserAnswer?> GetByQuizAttemptAndQuestionAsync(Guid quizAttemptId, Guid questionId, CancellationToken cancellationToken = default);
    Task<IEnumerable<UserAnswer>> GetCorrectAnswersForAttemptAsync(Guid quizAttemptId, CancellationToken cancellationToken = default);
    Task<IEnumerable<UserAnswer>> GetIncorrectAnswersForAttemptAsync(Guid quizAttemptId, CancellationToken cancellationToken = default);
    Task<int> GetTotalPointsForAttemptAsync(Guid quizAttemptId, CancellationToken cancellationToken = default);
    Task<int> CountCorrectAnswersForAttemptAsync(Guid quizAttemptId, CancellationToken cancellationToken = default);
    Task<int> CountIncorrectAnswersForAttemptAsync(Guid quizAttemptId, CancellationToken cancellationToken = default);
    Task<bool> HasAnsweredQuestionAsync(Guid quizAttemptId, Guid questionId, CancellationToken cancellationToken = default);
}
