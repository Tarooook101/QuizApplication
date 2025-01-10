using QuizApplication.DAL.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizApplication.BLL.DTOs
{
    public class QuizStatistics
    {
        public int TotalAttempts { get; set; }
        public double AverageScore { get; set; }
        public int PassCount { get; set; }
        public int FailCount { get; set; }
        public double PassRate => TotalAttempts > 0 ? (double)PassCount / TotalAttempts * 100 : 0;
        public TimeSpan AverageCompletionTime { get; set; }
        public Dictionary<QuestionType, int> QuestionTypeDistribution { get; set; } = new();
    }
}
