using QuizApplication.DAL.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizApplication.DAL.Entities
{
    public class QuizAttempt : BaseEntity<int>
    {
        public required string UserId { get; set; }
        public required int QuizId { get; set; }
        public DateTimeOffset StartedAt { get; set; }
        public DateTimeOffset? CompletedAt { get; set; }
        public int Score { get; set; }
        public QuizAttemptStatus Status { get; set; }
        public TimeSpan? Duration => CompletedAt?.Subtract(StartedAt);

        public virtual ApplicationUser User { get; set; } = null!;
        public virtual Quiz Quiz { get; set; } = null!;
        public virtual ICollection<QuestionResponse> Responses { get; set; } = new HashSet<QuestionResponse>();

        public bool IsCompleted => Status == QuizAttemptStatus.Completed;
        public bool IsPassed => Score >= Quiz.PassingScore;
    }
}
