using QuizApplication.DAL.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizApplication.DAL.Entities
{
    public class Option : BaseEntity<int>
    {
        public required int QuestionId { get; set; }
        public required string Text { get; set; }
        public bool IsCorrect { get; set; }
        public int DisplayOrder { get; set; }
        public string? Explanation { get; set; }

        public virtual Question Question { get; set; } = null!;
    }
}
