using Microsoft.EntityFrameworkCore;
using QuizApp.Domain.Entities;
using QuizApp.Domain.Repositories;


namespace QuizApp.Infrastructure.Persistence.Repositories;

public class QuestionRepository : Repository<Question, Guid>, IQuestionRepository
{
    public QuestionRepository(QuizDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Question>> GetByQuizIdAsync(Guid quizId, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Where(q => q.QuizId == quizId)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Question>> GetByQuizIdOrderedAsync(Guid quizId, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Where(q => q.QuizId == quizId)
            .OrderBy(q => q.OrderIndex)
            .ToListAsync(cancellationToken);
    }

    public async Task<Question?> GetByQuizIdAndOrderAsync(Guid quizId, int orderIndex, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .FirstOrDefaultAsync(q => q.QuizId == quizId && q.OrderIndex == orderIndex, cancellationToken);
    }

    public async Task<int> GetMaxOrderIndexByQuizIdAsync(Guid quizId, CancellationToken cancellationToken = default)
    {
        var maxOrder = await DbSet
            .Where(q => q.QuizId == quizId)
            .MaxAsync(q => (int?)q.OrderIndex, cancellationToken);

        return maxOrder ?? -1;
    }

    public async Task<bool> ExistsInQuizAsync(Guid questionId, Guid quizId, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .AnyAsync(q => q.Id == questionId && q.QuizId == quizId, cancellationToken);
    }

    public async Task<int> CountByQuizIdAsync(Guid quizId, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .CountAsync(q => q.QuizId == quizId, cancellationToken);
    }
}