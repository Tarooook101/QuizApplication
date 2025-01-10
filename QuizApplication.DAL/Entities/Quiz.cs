using QuizApplication.DAL.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizApplication.DAL.Entities
{
    public class Quiz : BaseEntity<int>
    {
        private const int DEFAULT_MAX_ATTEMPTS = 3;

        public required string Title { get; set; }
        public required string Description { get; set; }
        public TimeSpan Duration { get; set; }
        public DateTimeOffset StartDate { get; set; }
        public DateTimeOffset? EndDate { get; set; }
        public QuizStatus Status { get; set; }
        public QuizDifficulty Difficulty { get; set; }
        public int PassingScore { get; set; }
        public bool ShuffleQuestions { get; set; }
        public bool ShowResults { get; set; }
        public int MaxAttempts { get; set; } = DEFAULT_MAX_ATTEMPTS;
        public TimeSpan? TimeBetweenAttempts { get; set; }

        // Encapsulated AccessControl as a value object
        private AccessControl _accessControl = new();
        public AccessControl AccessControl
        {
            get => _accessControl;
            set => _accessControl = value ?? new AccessControl();
        }

        // Navigation properties
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
    }

}
