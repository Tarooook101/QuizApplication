using Microsoft.EntityFrameworkCore;
using QuizApplication.DAL.Common;
using QuizApplication.DAL.Database;
using QuizApplication.DAL.Entities;
using QuizApplication.DAL.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizApplication.DAL.Repositories
{
    public class QuizRepository : Repository<Quiz, int>, IQuizRepository
    {
        public QuizRepository(ApplicationDbContext context) : base(context) { }

        public async Task<IReadOnlyList<Quiz>> GetActiveQuizzesAsync(CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Where(q => q.Status == QuizStatus.Published &&
                           q.StartDate <= DateTimeOffset.UtcNow &&
                           (!q.EndDate.HasValue || q.EndDate > DateTimeOffset.UtcNow))
                .Include(q => q.Settings)
                .Include(q => q.Categories)
                .Include(q => q.Tags)
                .OrderBy(q => q.StartDate)
                .ToListAsync(cancellationToken);
        }

        public async Task<IReadOnlyList<Quiz>> GetQuizzesByUserAsync(string userId, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Where(q => q.CreatedById == userId)
                .Include(q => q.Settings)
                .Include(q => q.Categories)
                .OrderByDescending(q => q.CreatedAt)
                .ToListAsync(cancellationToken);
        }

        public async Task<bool> IsUserEligibleForQuizAsync(string userId, int quizId, CancellationToken cancellationToken = default)
        {
            var quiz = await _dbSet
                .Include(q => q.Attempts.Where(a => a.UserId == userId))
                .FirstOrDefaultAsync(q => q.Id == quizId, cancellationToken);

            if (quiz == null) return false;

            return quiz.CanAttempt(userId, quiz.Attempts.Count);
        }

        public override async Task<Quiz?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Include(q => q.Settings)
                .Include(q => q.Categories)
                .Include(q => q.Tags)
                .Include(q => q.Questions)
                    .ThenInclude(q => q.Options)
                .FirstOrDefaultAsync(q => q.Id == id, cancellationToken);
        }

    }
}
