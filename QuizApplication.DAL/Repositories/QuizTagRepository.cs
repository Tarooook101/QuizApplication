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
    public class QuizTagRepository : Repository<QuizTag, int>, IQuizTagRepository
    {
        public QuizTagRepository(ApplicationDbContext context) : base(context) { }

        public async Task<IReadOnlyList<QuizTag>> GetByQuizIdAsync(int quizId, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Where(qt => qt.Quizzes.Any(q => q.Id == quizId))
                .ToListAsync(cancellationToken);
        }

        public async Task<IReadOnlyList<Quiz>> GetQuizzesByTagAsync(int tagId, CancellationToken cancellationToken = default)
        {
            var tag = await _dbSet
                .Include(t => t.Quizzes)
                    .ThenInclude(q => q.Settings)
                .Include(t => t.Quizzes)
                    .ThenInclude(q => q.Categories)
                .FirstOrDefaultAsync(t => t.Id == tagId, cancellationToken);

            return tag?.Quizzes.OrderByDescending(q => q.CreatedAt).ToList() ?? new List<Quiz>();
        }
    }
}
