using QuizApplication.BLL.DTOs;
using QuizApplication.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizApplication.BLL.Interfaces
{
    public interface IUserService
    {
        Task<ApplicationUser> GetUserByIdAsync(string userId, CancellationToken cancellationToken = default);
        Task<ApplicationUser> GetUserByEmailAsync(string email, CancellationToken cancellationToken = default);
        Task<UserProfile> GetUserProfileAsync(string userId, CancellationToken cancellationToken = default);
        Task UpdateUserProfileAsync(string userId, UserProfile profile, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<UserAchievement>> GetUserAchievementsAsync(string userId, CancellationToken cancellationToken = default);
        Task<UserStatistics> GetUserStatisticsAsync(string userId, CancellationToken cancellationToken = default);
    }
}
