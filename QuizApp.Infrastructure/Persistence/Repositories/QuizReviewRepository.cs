using Microsoft.EntityFrameworkCore;
using QuizApp.Domain.Entities;
using QuizApp.Domain.Repositories;

namespace QuizApp.Infrastructure.Persistence.Repositories;

public class QuizReviewRepository : Repository<QuizReview, Guid>, IQuizReviewRepository
{
    public QuizReviewRepository(QuizDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<QuizReview>> GetReviewsByQuizAsync(Guid quizId, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Where(r => r.QuizId == quizId)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<QuizReview>> GetReviewsByUserAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Where(r => r.UserId == userId)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<QuizReview>> GetPublicReviewsByQuizAsync(Guid quizId, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Where(r => r.QuizId == quizId && r.IsPublic)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<QuizReview?> GetUserReviewForQuizAsync(Guid userId, Guid quizId, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .FirstOrDefaultAsync(r => r.UserId == userId && r.QuizId == quizId, cancellationToken);
    }

    public async Task<double> GetAverageRatingForQuizAsync(Guid quizId, CancellationToken cancellationToken = default)
    {
        var reviews = await DbSet
            .Where(r => r.QuizId == quizId && r.IsPublic)
            .ToListAsync(cancellationToken);

        return reviews.Any() ? reviews.Average(r => r.Rating) : 0;
    }

    public async Task<int> GetReviewCountForQuizAsync(Guid quizId, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .CountAsync(r => r.QuizId == quizId && r.IsPublic, cancellationToken);
    }

    public async Task<bool> HasUserReviewedQuizAsync(Guid userId, Guid quizId, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .AnyAsync(r => r.UserId == userId && r.QuizId == quizId, cancellationToken);
    }
}