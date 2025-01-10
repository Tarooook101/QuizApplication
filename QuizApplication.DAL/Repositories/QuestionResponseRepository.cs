using Microsoft.EntityFrameworkCore;
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
    public class QuestionResponseRepository : Repository<QuestionResponse, int>, IQuestionResponseRepository
    {
        public QuestionResponseRepository(ApplicationDbContext context) : base(context) { }

        public async Task<IReadOnlyList<QuestionResponse>> GetByAttemptIdAsync(int attemptId, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Where(qr => qr.QuizAttemptId == attemptId)
                .Include(qr => qr.Question)
                    .ThenInclude(q => q.Options)
                .OrderBy(qr => qr.Question.DisplayOrder)
                .ToListAsync(cancellationToken);
        }

        public async Task<bool> HasUserAnsweredQuestionAsync(string userId, int questionId, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .AnyAsync(qr => qr.QuizAttempt.UserId == userId &&
                               qr.QuestionId == questionId,
                         cancellationToken);
        }

        public override async Task<QuestionResponse?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Include(qr => qr.Question)
                    .ThenInclude(q => q.Options)
                .Include(qr => qr.QuizAttempt)
                .FirstOrDefaultAsync(qr => qr.Id == id, cancellationToken);
        }
    }

}
