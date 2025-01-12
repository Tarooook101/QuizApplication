using QuizApplication.DAL.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace QuizApplication.DAL.Entities
{
    public class Quiz : BaseEntity<int>
    {
        private const int DEFAULT_MAX_ATTEMPTS = 3;

        public required string Title { get; set; }
        public required string Description { get; set; }
        public int DurationMinutes { get; set; }  // Changed from TimeSpan to int
        public DateTimeOffset StartDate { get; set; }
        public DateTimeOffset? EndDate { get; set; }
        public QuizStatus Status { get; set; }
        public QuizDifficulty Difficulty { get; set; }
        public int PassingScore { get; set; }
        public bool ShuffleQuestions { get; set; }
        public bool ShowResults { get; set; }
        public int MaxAttempts { get; set; } = DEFAULT_MAX_ATTEMPTS;
        public int? TimeBetweenAttemptsMinutes { get; set; }  // Changed from TimeSpan to int?

        private string _accessControlJson = JsonSerializer.Serialize(new AccessControl());
        private AccessControl? _accessControl;

        public AccessControl AccessControl
        {
            get => _accessControl ??= JsonSerializer.Deserialize<AccessControl>(_accessControlJson) ?? new AccessControl();
            set
            {
                _accessControl = value;
                _accessControlJson = JsonSerializer.Serialize(value);
            }
        }

        // Navigation properties remain the same
        public required string CreatedById { get; set; }
        public virtual ApplicationUser CreatedBy { get; set; } = null!;
        public virtual ICollection<Question> Questions { get; set; } = new HashSet<Question>();
        public virtual ICollection<QuizAttempt> Attempts { get; set; } = new HashSet<QuizAttempt>();
        public virtual ICollection<Category> Categories { get; set; } = new HashSet<Category>();
        public virtual ICollection<QuizTag> Tags { get; set; } = new HashSet<QuizTag>();
        public virtual QuizSettings Settings { get; set; } = null!;

        // Domain logic methods
        public bool IsActive => Status == QuizStatus.Published &&
                              StartDate <= DateTimeOffset.UtcNow &&
                              (!EndDate.HasValue || EndDate > DateTimeOffset.UtcNow);

        public bool CanAttempt(string userId, int attemptCount) =>
            IsActive &&
            attemptCount < MaxAttempts &&
            AccessControl.HasAccess(userId);

        // Helper methods for TimeSpan conversion
        public TimeSpan Duration => TimeSpan.FromMinutes(DurationMinutes);
        public TimeSpan? TimeBetweenAttempts => TimeBetweenAttemptsMinutes.HasValue
            ? TimeSpan.FromMinutes(TimeBetweenAttemptsMinutes.Value)
            : null;
    }


}
