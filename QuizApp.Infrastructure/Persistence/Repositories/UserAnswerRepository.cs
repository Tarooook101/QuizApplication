using Microsoft.EntityFrameworkCore;
using QuizApp.Domain.Entities;
using QuizApp.Domain.Repositories;


namespace QuizApp.Infrastructure.Persistence.Repositories;

public class UserAnswerRepository : Repository<UserAnswer, Guid>, IUserAnswerRepository
{
    public UserAnswerRepository(QuizDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<UserAnswer>> GetByQuizAttemptIdAsync(Guid quizAttemptId, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Include(x => x.Question)
            .Include(x => x.SelectedAnswer)
            .Include(x => x.QuizAttempt)
            .Where(x => x.QuizAttemptId == quizAttemptId)
            .OrderBy(x => x.Question.OrderIndex)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<UserAnswer>> GetByQuestionIdAsync(Guid questionId, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Include(x => x.Question)
            .Include(x => x.SelectedAnswer)
            .Include(x => x.QuizAttempt)
            .Where(x => x.QuestionId == questionId)
            .OrderByDescending(x => x.AnsweredAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<UserAnswer?> GetByQuizAttemptAndQuestionAsync(Guid quizAttemptId, Guid questionId, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Include(x => x.Question)
            .Include(x => x.SelectedAnswer)
            .Include(x => x.QuizAttempt)
            .FirstOrDefaultAsync(x => x.QuizAttemptId == quizAttemptId && x.QuestionId == questionId, cancellationToken);
    }

    public async Task<IEnumerable<UserAnswer>> GetCorrectAnswersForAttemptAsync(Guid quizAttemptId, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Include(x => x.Question)
            .Include(x => x.SelectedAnswer)
            .Where(x => x.QuizAttemptId == quizAttemptId && x.IsCorrect)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<UserAnswer>> GetIncorrectAnswersForAttemptAsync(Guid quizAttemptId, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Include(x => x.Question)
            .Include(x => x.SelectedAnswer)
            .Where(x => x.QuizAttemptId == quizAttemptId && !x.IsCorrect)
            .ToListAsync(cancellationToken);
    }

    public async Task<int> GetTotalPointsForAttemptAsync(Guid quizAttemptId, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Where(x => x.QuizAttemptId == quizAttemptId)
            .SumAsync(x => x.PointsEarned, cancellationToken);
    }

    public async Task<int> CountCorrectAnswersForAttemptAsync(Guid quizAttemptId, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .CountAsync(x => x.QuizAttemptId == quizAttemptId && x.IsCorrect, cancellationToken);
    }

    public async Task<int> CountIncorrectAnswersForAttemptAsync(Guid quizAttemptId, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .CountAsync(x => x.QuizAttemptId == quizAttemptId && !x.IsCorrect, cancellationToken);
    }

    public async Task<bool> HasAnsweredQuestionAsync(Guid quizAttemptId, Guid questionId, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .AnyAsync(x => x.QuizAttemptId == quizAttemptId && x.QuestionId == questionId, cancellationToken);
    }
}