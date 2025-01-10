using QuizApplication.DAL.Common;
using QuizApplication.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizApplication.DAL.Interfaces
{
    public interface IUserProfileRepository : IRepository<UserProfile, int>
    {
        Task<UserProfile> GetByUserIdAsync(string userId, CancellationToken cancellationToken = default);
        Task UpdateNotificationPreferencesAsync(string userId, NotificationPreferences preferences, CancellationToken cancellationToken = default);
    }
}
