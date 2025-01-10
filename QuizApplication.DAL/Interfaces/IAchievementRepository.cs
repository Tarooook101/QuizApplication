using QuizApplication.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizApplication.DAL.Interfaces
{
    public interface IAchievementRepository : IRepository<Achievement, int>
    {
        Task<IReadOnlyList<Achievement>> GetAvailableAchievementsAsync(string userId, CancellationToken cancellationToken = default);
        Task<bool> CheckEligibilityAsync(string userId, int achievementId, CancellationToken cancellationToken = default);
    }
}
