using QuizApplication.DAL.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizApplication.DAL.Entities
{
    public class QuestionResponse : BaseEntity<int>
    {
        public required int QuestionId { get; set; }
        public required int QuizAttemptId { get; set; }
        public string? Response { get; set; }
        public bool IsCorrect { get; set; }
        public int ScoreEarned { get; set; }
        public TimeSpan? TimeSpent { get; set; }

        public virtual Question Question { get; set; } = null!;
        public virtual QuizAttempt QuizAttempt { get; set; } = null!;
    }
}
