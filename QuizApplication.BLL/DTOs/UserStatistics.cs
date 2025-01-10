using QuizApplication.DAL.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizApplication.BLL.DTOs
{
    public class UserStatistics
    {
        public int TotalQuizAttempts { get; set; }
        public int CompletedQuizzes { get; set; }
        public double AverageScore { get; set; }
        public int TotalAchievements { get; set; }
        public TimeSpan AverageQuizCompletionTime { get; set; }
        public Dictionary<QuizDifficulty, int> CompletedQuizzesByDifficulty { get; set; } = new();
    }
}
