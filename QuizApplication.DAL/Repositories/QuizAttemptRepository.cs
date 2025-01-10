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
    public class QuizAttemptRepository : Repository<QuizAttempt, int>, IQuizAttemptRepository
    {
        public QuizAttemptRepository(ApplicationDbContext context) : base(context) { }

        public async Task<IReadOnlyList<QuizAttempt>> GetUserAttemptsAsync(string userId, int quizId, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Where(qa => qa.UserId == userId && qa.QuizId == quizId)
                .Include(qa => qa.Responses)
                .OrderByDescending(qa => qa.StartedAt)
                .ToListAsync(cancellationToken);
        }

        public async Task<int> GetAttemptCountAsync(string userId, int quizId, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .CountAsync(qa => qa.UserId == userId &&
                                 qa.QuizId == quizId &&
                                 qa.Status != QuizAttemptStatus.Abandoned,
                           cancellationToken);
        }

        public override async Task<QuizAttempt?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Include(qa => qa.Quiz)
                .Include(qa => qa.Responses)
                    .ThenInclude(qr => qr.Question)
                        .ThenInclude(q => q.Options)
                .FirstOrDefaultAsync(qa => qa.Id == id, cancellationToken);
        }

    }
}
