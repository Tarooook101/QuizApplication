using Microsoft.EntityFrameworkCore;
using QuizApplication.DAL.Common;
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
    public class UserProfileRepository : Repository<UserProfile, int>, IUserProfileRepository
    {
        public UserProfileRepository(ApplicationDbContext context) : base(context) { }

        public async Task<UserProfile> GetByUserIdAsync(string userId, CancellationToken cancellationToken = default)
        {
            var profile = await _dbSet
                .Include(up => up.User)
                .FirstOrDefaultAsync(up => up.UserId == userId, cancellationToken);

            if (profile == null)
            {
                profile = new UserProfile
                {
                    UserId = userId,
                    TimeZone = "UTC",
                    Language = "en",
                    NotificationPreferences = new NotificationPreferences()
                };
                await AddAsync(profile, cancellationToken);
            }

            return profile;
        }

        public async Task UpdateNotificationPreferencesAsync(
            string userId,
            NotificationPreferences preferences,
            CancellationToken cancellationToken = default)
        {
            var profile = await GetByUserIdAsync(userId, cancellationToken);
            var entry = _context.Entry(profile);

            entry.Property(p => p.NotificationPreferences).CurrentValue = preferences;
            await UpdateAsync(profile, cancellationToken);
        }
    }
}
