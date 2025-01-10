using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizApplication.BLL.DTOs
{
    public class QuestionStatistics
    {
        public int TotalResponses { get; set; }
        public int CorrectResponses { get; set; }
        public double CorrectResponseRate => TotalResponses > 0 ? (double)CorrectResponses / TotalResponses * 100 : 0;
        public TimeSpan AverageResponseTime { get; set; }
        public Dictionary<string, int> AnswerDistribution { get; set; } = new();
    }
}
