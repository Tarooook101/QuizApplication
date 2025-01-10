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
    public class OptionRepository : Repository<Option, int>, IOptionRepository
    {
        public OptionRepository(ApplicationDbContext context) : base(context) { }

        public async Task<IReadOnlyList<Option>> GetByQuestionIdAsync(int questionId, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Where(o => o.QuestionId == questionId)
                .OrderBy(o => o.DisplayOrder)
                .ToListAsync(cancellationToken);
        }

        public async Task UpdateOptionOrdersAsync(
            IEnumerable<(int OptionId, int NewOrder)> optionOrders,
            CancellationToken cancellationToken = default)
        {
            foreach (var (optionId, newOrder) in optionOrders)
            {
                var option = await _dbSet.FindAsync(new object[] { optionId }, cancellationToken);
                if (option != null)
                {
                    option.DisplayOrder = newOrder;
                    _dbSet.Update(option);
                }
            }
        }

        public override async Task<Option?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Include(o => o.Question)
                .FirstOrDefaultAsync(o => o.Id == id, cancellationToken);
        }
    }
}
