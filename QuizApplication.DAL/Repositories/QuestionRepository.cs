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
    public class QuestionRepository : Repository<Question, int>, IQuestionRepository
    {
        public QuestionRepository(ApplicationDbContext context) : base(context) { }

        public async Task<IReadOnlyList<Question>> GetByQuizIdAsync(
            int quizId,
            bool includeOptions = true,
            CancellationToken cancellationToken = default)
        {
            var query = _dbSet
                .Where(q => q.QuizId == quizId)
                .Include(q => q.Metadata);

            if (includeOptions)
            {
                query = (Microsoft.EntityFrameworkCore.Query.IIncludableQueryable<Question, QuestionMetadata>)query.Include(q => q.Options.OrderBy(o => o.DisplayOrder));
            }

            return await query
                .OrderBy(q => q.DisplayOrder)
                .ToListAsync(cancellationToken);
        }

        public async Task UpdateQuestionOrdersAsync(
            IEnumerable<(int QuestionId, int NewOrder)> questionOrders,
            CancellationToken cancellationToken = default)
        {
            foreach (var (questionId, newOrder) in questionOrders)
            {
                var question = await _dbSet.FindAsync(new object[] { questionId }, cancellationToken);
                if (question != null)
                {
                    question.DisplayOrder = newOrder;
                    _dbSet.Update(question);
                }
            }
        }

        public override async Task<Question?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Include(q => q.Options.OrderBy(o => o.DisplayOrder))
                .Include(q => q.Metadata)
                .FirstOrDefaultAsync(q => q.Id == id, cancellationToken);
        }
    }
}
