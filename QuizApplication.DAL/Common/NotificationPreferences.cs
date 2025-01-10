using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizApplication.DAL.Common
{
    public record NotificationPreferences
    {
        public bool EmailNotifications { get; init; }
        public bool PushNotifications { get; init; }
        public bool QuizReminders { get; init; }
        public bool AchievementNotifications { get; init; }
        public TimeSpan? QuietHoursStart { get; init; }
        public TimeSpan? QuietHoursEnd { get; init; }

        public bool IsInQuietHours(TimeSpan currentTime)
        {
            if (!QuietHoursStart.HasValue || !QuietHoursEnd.HasValue)
                return false;

            if (QuietHoursStart.Value <= QuietHoursEnd.Value)
            {
                return currentTime >= QuietHoursStart.Value && currentTime <= QuietHoursEnd.Value;
            }

            return currentTime >= QuietHoursStart.Value || currentTime <= QuietHoursEnd.Value;
        }
    }
}
