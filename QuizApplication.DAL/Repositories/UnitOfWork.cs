using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using QuizApplication.DAL.Database;
using QuizApplication.DAL.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizApplication.DAL.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;
        private readonly Dictionary<Type, object> _repositories;
        private IDbContextTransaction? _transaction;
        private bool _disposed;

        // Lazy-loaded repositories
        private Lazy<IQuizRepository> _quizzes;
        private Lazy<IQuestionRepository> _questions;
        private Lazy<IOptionRepository> _options;
        private Lazy<IQuizAttemptRepository> _quizAttempts;
        private Lazy<IQuestionResponseRepository> _questionResponses;
        private Lazy<ICategoryRepository> _categories;
        private Lazy<IQuizTagRepository> _tags;
        private Lazy<IUserProfileRepository> _userProfiles;
        private Lazy<IUserRepository> _users;
        private Lazy<IAchievementRepository> _achievements;
        private Lazy<IUserAchievementRepository> _userAchievements;

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _repositories = new Dictionary<Type, object>();

            // Initialize lazy-loaded repositories
            InitializeLazyRepositories();
        }

        private void InitializeLazyRepositories()
        {
            _quizzes = new Lazy<IQuizRepository>(() => new QuizRepository(_context));
            _questions = new Lazy<IQuestionRepository>(() => new QuestionRepository(_context));
            _options = new Lazy<IOptionRepository>(() => new OptionRepository(_context));
            _quizAttempts = new Lazy<IQuizAttemptRepository>(() => new QuizAttemptRepository(_context));
            _questionResponses = new Lazy<IQuestionResponseRepository>(() => new QuestionResponseRepository(_context));
            _categories = new Lazy<ICategoryRepository>(() => new CategoryRepository(_context));
            _tags = new Lazy<IQuizTagRepository>(() => new QuizTagRepository(_context));
            _userProfiles = new Lazy<IUserProfileRepository>(() => new UserProfileRepository(_context));
            _users = new Lazy<IUserRepository>(() => new UserRepository(_context));
            _achievements = new Lazy<IAchievementRepository>(() => new AchievementRepository(_context));
            _userAchievements = new Lazy<IUserAchievementRepository>(() => new UserAchievementRepository(_context));
        }

        // Repository Properties with Lazy Loading
        public IQuizRepository Quizzes => _quizzes.Value;
        public IQuestionRepository Questions => _questions.Value;
        public IOptionRepository Options => _options.Value;
        public IQuizAttemptRepository QuizAttempts => _quizAttempts.Value;
        public IQuestionResponseRepository QuestionResponses => _questionResponses.Value;
        public ICategoryRepository Categories => _categories.Value;
        public IQuizTagRepository Tags => _tags.Value;
        public IUserProfileRepository UserProfiles => _userProfiles.Value;
        public IUserRepository Users => _users.Value;
        public IAchievementRepository Achievements => _achievements.Value;
        public IUserAchievementRepository UserAchievements => _userAchievements.Value;

        // Generic Repository Factory Method
        public IRepository<TEntity, TKey> Repository<TEntity, TKey>() where TEntity : class
        {
            var type = typeof(TEntity);

            if (!_repositories.ContainsKey(type))
            {
                var repositoryType = typeof(Repository<,>).MakeGenericType(typeof(TEntity), typeof(TKey));
                var repository = Activator.CreateInstance(repositoryType, _context);
                _repositories.Add(type, repository!);
            }

            return (IRepository<TEntity, TKey>)_repositories[type];
        }

        // Transaction Management
        public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
        {
            if (_transaction != null)
            {
                throw new InvalidOperationException("A transaction is already in progress");
            }

            _transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
        }

        public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                await SaveChangesAsync(cancellationToken);

                if (_transaction != null)
                {
                    await _transaction.CommitAsync(cancellationToken);
                }
            }
            catch
            {
                await RollbackTransactionAsync(cancellationToken);
                throw;
            }
            finally
            {
                if (_transaction != null)
                {
                    await _transaction.DisposeAsync();
                    _transaction = null;
                }
            }
        }

        public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
        {
            if (_transaction != null)
            {
                await _transaction.RollbackAsync(cancellationToken);
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        // Execution Strategy
        public Task<IExecutionStrategy> CreateExecutionStrategy()
        {
            return Task.FromResult(_context.Database.CreateExecutionStrategy());
        }

        // Save Changes Methods
        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task<int> SaveChangesWithoutEventsAsync(CancellationToken cancellationToken = default)
        {
            using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
            try
            {
                var result = await _context.SaveChangesAsync(cancellationToken);
                await transaction.CommitAsync(cancellationToken);
                return result;
            }
            catch
            {
                await transaction.RollbackAsync(cancellationToken);
                throw;
            }
        }

        // Disposal Pattern Implementation
        protected virtual async ValueTask DisposeManagedResourcesAsync()
        {
            if (_transaction != null)
            {
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        public async ValueTask DisposeAsync()
        {
            if (!_disposed)
            {
                await DisposeManagedResourcesAsync();
                _disposed = true;
            }
            GC.SuppressFinalize(this);
        }
    }
}
