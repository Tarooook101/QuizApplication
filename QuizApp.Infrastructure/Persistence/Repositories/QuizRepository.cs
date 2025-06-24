using Microsoft.EntityFrameworkCore;
using QuizApp.Domain.Entities;
using QuizApp.Domain.Enums;
using QuizApp.Domain.Repositories;


namespace QuizApp.Infrastructure.Persistence.Repositories;

public class QuizRepository : Repository<Quiz, Guid>, IQuizRepository
{
    public QuizRepository(QuizDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Quiz>> GetActiveQuizzesAsync(CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Where(q => q.IsActive)
            .OrderByDescending(q => q.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Quiz>> GetPublicQuizzesAsync(CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Where(q => q.IsPublic && q.IsActive)
            .OrderByDescending(q => q.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Quiz>> GetQuizzesByUserAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Where(q => q.CreatedByUserId == userId)
            .OrderByDescending(q => q.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Quiz>> GetQuizzesByDifficultyAsync(QuizDifficulty difficulty, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Where(q => q.Difficulty == difficulty && q.IsActive && q.IsPublic)
            .OrderByDescending(q => q.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> ExistsByTitleAsync(string title, Guid? excludeId = null, CancellationToken cancellationToken = default)
    {
        var query = DbSet.Where(q => q.Title.ToLower() == title.ToLower());

        if (excludeId.HasValue)
        {
            query = query.Where(q => q.Id != excludeId.Value);
        }

        return await query.AnyAsync(cancellationToken);
    }
}