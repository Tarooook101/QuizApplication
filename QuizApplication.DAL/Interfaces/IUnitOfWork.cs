using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizApplication.DAL.Interfaces
{
    public interface IUnitOfWork : IAsyncDisposable
    {
        // Generic repository accessor
        IRepository<TEntity, TKey> Repository<TEntity, TKey>() where TEntity : class;

        // Specialized repositories
        IQuizRepository Quizzes { get; }
        IQuestionRepository Questions { get; }
        IOptionRepository Options { get; }
        IQuizAttemptRepository QuizAttempts { get; }
        IQuestionResponseRepository QuestionResponses { get; }
        ICategoryRepository Categories { get; }
        IQuizTagRepository Tags { get; }
        IUserProfileRepository UserProfiles { get; }
        IUserRepository Users { get; }
        IAchievementRepository Achievements { get; }
        IUserAchievementRepository UserAchievements { get; }

        // Transaction management
        Task BeginTransactionAsync(CancellationToken cancellationToken = default);
        Task CommitTransactionAsync(CancellationToken cancellationToken = default);
        Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
        Task<IExecutionStrategy> CreateExecutionStrategy();

        // Save changes
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
        Task<int> SaveChangesWithoutEventsAsync(CancellationToken cancellationToken = default);
    }
}
