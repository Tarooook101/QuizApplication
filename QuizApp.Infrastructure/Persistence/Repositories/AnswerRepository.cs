using Microsoft.EntityFrameworkCore;
using QuizApp.Domain.Entities;
using QuizApp.Domain.Repositories;

namespace QuizApp.Infrastructure.Persistence.Repositories;

public class AnswerRepository : Repository<Answer, Guid>, IAnswerRepository
{
    public AnswerRepository(QuizDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Answer>> GetByQuestionIdAsync(Guid questionId, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Where(a => a.QuestionId == questionId)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Answer>> GetByQuestionIdOrderedAsync(Guid questionId, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Where(a => a.QuestionId == questionId)
            .OrderBy(a => a.OrderIndex)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Answer>> GetCorrectAnswersByQuestionIdAsync(Guid questionId, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Where(a => a.QuestionId == questionId && a.IsCorrect)
            .OrderBy(a => a.OrderIndex)
            .ToListAsync(cancellationToken);
    }

    public async Task<Answer?> GetByQuestionIdAndOrderAsync(Guid questionId, int orderIndex, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .FirstOrDefaultAsync(a => a.QuestionId == questionId && a.OrderIndex == orderIndex, cancellationToken);
    }

    public async Task<int> GetMaxOrderIndexByQuestionIdAsync(Guid questionId, CancellationToken cancellationToken = default)
    {
        var maxOrder = await DbSet
            .Where(a => a.QuestionId == questionId)
            .MaxAsync(a => (int?)a.OrderIndex, cancellationToken);

        return maxOrder ?? -1;
    }

    public async Task<bool> ExistsInQuestionAsync(Guid answerId, Guid questionId, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .AnyAsync(a => a.Id == answerId && a.QuestionId == questionId, cancellationToken);
    }

    public async Task<int> CountByQuestionIdAsync(Guid questionId, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .CountAsync(a => a.QuestionId == questionId, cancellationToken);
    }

    public async Task<int> CountCorrectAnswersByQuestionIdAsync(Guid questionId, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .CountAsync(a => a.QuestionId == questionId && a.IsCorrect, cancellationToken);
    }
}
