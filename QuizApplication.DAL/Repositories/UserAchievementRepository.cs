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
    public class UserAchievementRepository : Repository<UserAchievement, int>, IUserAchievementRepository
    {
        public UserAchievementRepository(ApplicationDbContext context) : base(context) { }

        public async Task<IReadOnlyList<UserAchievement>> GetByUserIdAsync(string userId, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Where(ua => ua.UserId == userId)
                .Include(ua => ua.Achievement)
                .OrderByDescending(ua => ua.AwardedDate)
                .ToListAsync(cancellationToken);
        }

        public async Task<bool> HasAchievementAsync(string userId, int achievementId, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .AnyAsync(ua => ua.UserId == userId &&
                               ua.AchievementId == achievementId,
                         cancellationToken);
        }
    }
}
