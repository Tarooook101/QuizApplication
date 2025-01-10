using QuizApplication.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizApplication.DAL.Interfaces
{
    public interface IUserAchievementRepository : IRepository<UserAchievement, int>
    {
        Task<IReadOnlyList<UserAchievement>> GetByUserIdAsync(string userId, CancellationToken cancellationToken = default);
        Task<bool> HasAchievementAsync(string userId, int achievementId, CancellationToken cancellationToken = default);
    }
}
