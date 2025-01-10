using QuizApplication.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizApplication.BLL.DTOs
{
    public class QuizAttemptResult
    {
        public int AttemptId { get; set; }
        public int Score { get; set; }
        public bool IsPassed { get; set; }
        public TimeSpan TimeTaken { get; set; }
        public int CorrectAnswers { get; set; }
        public int TotalQuestions { get; set; }
        public double ScorePercentage => TotalQuestions > 0 ? (double)Score / TotalQuestions * 100 : 0;
        public List<QuestionResponse> Responses { get; set; } = new();
    }
}
