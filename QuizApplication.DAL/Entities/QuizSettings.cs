using QuizApplication.DAL.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizApplication.DAL.Entities
{
    public class QuizSettings : BaseEntity<int>
    {
        public required int QuizId { get; set; }
        public bool AllowReview { get; set; }
        public bool ShowCorrectAnswers { get; set; }
        public bool RequireAuthentication { get; set; }
        public bool EnableAnalytics { get; set; }
        public int? TimeLimit { get; set; }

        private Dictionary<string, string> _customSettings = new();
        public Dictionary<string, string> CustomSettings
        {
            get => new(_customSettings);
            set => _customSettings = value ?? new Dictionary<string, string>();
        }

        public virtual Quiz Quiz { get; set; } = null!;

        public void UpdateSetting(string key, string value)
        {
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentException("Setting key cannot be empty", nameof(key));

            _customSettings[key] = value;
        }
    }
}
