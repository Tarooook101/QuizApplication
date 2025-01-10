using QuizApplication.DAL.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizApplication.DAL.Entities
{
    public class UserProfile : BaseEntity<int>
    {
        public required string UserId { get; set; }
        public required string TimeZone { get; set; } = "UTC";
        public required string Language { get; set; } = "en";
        public required NotificationPreferences NotificationPreferences { get; set; } = new();
        public string? Biography { get; set; }

        private Dictionary<string, string> _customSettings = new();
        public Dictionary<string, string> CustomSettings
        {
            get => new(_customSettings);
            set => _customSettings = value ?? new Dictionary<string, string>();
        }

        public virtual ApplicationUser User { get; set; } = null!;

        public void UpdateSetting(string key, string value)
        {
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentException("Setting key cannot be empty", nameof(key));

            _customSettings[key] = value;
        }
    }
}
