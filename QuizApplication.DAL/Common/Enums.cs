using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizApplication.DAL.Common
{
    public enum QuizAttemptStatus
    {
        Started,
        InProgress,
        Completed,
        Abandoned,
        TimedOut
    }

    public enum QuestionType
    {
        MultipleChoice,
        TrueFalse,
        ShortAnswer,
        Essay,
        Matching,
        Ordering
    }

    public enum QuizDifficulty
    {
        Beginner,
        Intermediate,
        Advanced,
        Expert
    }
    public enum AchievementType
    {
        QuizCompletion,
        HighScore,
        Streak,
        TopPerformer,
        FirstAttempt,
        PerfectScore,
        Participation,
        Custom
    }

    public enum UserRole
    {
        Student,
        Teacher,
        Administrator,
        ContentCreator,
        Moderator
    }

    public enum UserStatus
    {
        Active,
        Inactive,
        Suspended,
        PendingVerification,
        Archived,
        Pending
    }

    public enum QuestionDifficulty
    {
        Easy,
        Medium,
        Hard,
        Expert
    }

    public enum QuizStatus
    {
        Draft,
        Published,
        Archived,
        Scheduled,
        Closed
    }
}
