using QuizApplication.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizApplication.BLL.Interfaces
{
    public interface IAchievementService : IService<Achievement, int>
    {
        Task<IReadOnlyList<Achievement>> GetAvailableAchievementsAsync(string userId, CancellationToken cancellationToken = default);
        Task<UserAchievement> AwardAchievementAsync(string userId, int achievementId, CancellationToken cancellationToken = default);
        Task<bool> CheckUserEligibilityAsync(string userId, int achievementId, CancellationToken cancellationToken = default);
        Task ProcessQuizCompletionAchievementsAsync(string userId, int quizId, CancellationToken cancellationToken = default);
    }
}
