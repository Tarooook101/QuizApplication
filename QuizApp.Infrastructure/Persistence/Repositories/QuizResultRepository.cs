using Microsoft.EntityFrameworkCore;
using QuizApp.Domain.Entities;
using QuizApp.Domain.Enums;
using QuizApp.Domain.Repositories;


namespace QuizApp.Infrastructure.Persistence.Repositories;

public class QuizResultRepository : Repository<QuizResult, Guid>, IQuizResultRepository
{
    public QuizResultRepository(QuizDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<QuizResult>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Where(qr => qr.UserId == userId)
            .Include(qr => qr.User)
            .OrderByDescending(qr => qr.CompletedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<QuizResult>> GetByQuizIdAsync(Guid quizId, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Where(qr => qr.QuizId == quizId)
            .Include(qr => qr.User)
            .OrderByDescending(qr => qr.Score)
            .ThenByDescending(qr => qr.CompletedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<QuizResult>> GetPassedResultsAsync(CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Where(qr => qr.Status == QuizResultStatus.Passed)
            .Include(qr => qr.User)
            .OrderByDescending(qr => qr.CompletedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<QuizResult>> GetFailedResultsAsync(CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Where(qr => qr.Status == QuizResultStatus.Failed)
            .Include(qr => qr.User)
            .OrderByDescending(qr => qr.CompletedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<QuizResult?> GetByQuizAttemptIdAsync(Guid quizAttemptId, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Include(qr => qr.User)
            .Include(qr => qr.QuizAttempt)
            .FirstOrDefaultAsync(qr => qr.QuizAttemptId == quizAttemptId, cancellationToken);
    }

    public async Task<IEnumerable<QuizResult>> GetTopScoresAsync(Guid quizId, int count = 10, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Where(qr => qr.QuizId == quizId)
            .Include(qr => qr.User)
            .OrderByDescending(qr => qr.Score)
            .ThenBy(qr => qr.TimeSpent)
            .Take(count)
            .ToListAsync(cancellationToken);
    }

    public async Task<double> GetAverageScoreAsync(Guid quizId, CancellationToken cancellationToken = default)
    {
        var results = await DbSet
            .Where(qr => qr.QuizId == quizId)
            .ToListAsync(cancellationToken);

        return results.Any() ? results.Average(qr => qr.Percentage) : 0;
    }
}