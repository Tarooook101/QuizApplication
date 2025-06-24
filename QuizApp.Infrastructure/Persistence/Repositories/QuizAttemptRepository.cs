using Microsoft.EntityFrameworkCore;
using QuizApp.Domain.Entities;
using QuizApp.Domain.Enums;
using QuizApp.Domain.Repositories;


namespace QuizApp.Infrastructure.Persistence.Repositories;

public class QuizAttemptRepository : Repository<QuizAttempt, Guid>, IQuizAttemptRepository
{
    public QuizAttemptRepository(QuizDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<QuizAttempt>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Where(qa => qa.UserId == userId)
            .OrderByDescending(qa => qa.StartedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<QuizAttempt>> GetByQuizIdAsync(Guid quizId, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Where(qa => qa.QuizId == quizId)
            .OrderByDescending(qa => qa.StartedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<QuizAttempt>> GetCompletedAttemptsAsync(CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Where(qa => qa.Status == QuizAttemptStatus.Completed)
            .OrderByDescending(qa => qa.CompletedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<QuizAttempt>> GetInProgressAttemptsAsync(CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Where(qa => qa.Status == QuizAttemptStatus.InProgress)
            .OrderByDescending(qa => qa.StartedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<int> GetAttemptCountForUserAndQuizAsync(Guid userId, Guid quizId, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .CountAsync(qa => qa.UserId == userId && qa.QuizId == quizId, cancellationToken);
    }

    public async Task<QuizAttempt?> GetActiveAttemptAsync(Guid userId, Guid quizId, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .FirstOrDefaultAsync(qa => qa.UserId == userId && qa.QuizId == quizId && qa.Status == QuizAttemptStatus.InProgress, cancellationToken);
    }
}