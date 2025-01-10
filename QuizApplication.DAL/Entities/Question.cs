using QuizApplication.DAL.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizApplication.DAL.Entities
{
    public class Question : BaseEntity<int>
    {
        public required string Text { get; set; }
        public int Points { get; set; }
        public QuestionType Type { get; set; }
        public QuestionDifficulty Difficulty { get; set; }
        public string? ImageUrl { get; set; }
        public string? Explanation { get; set; }
        public bool IsMandatory { get; set; }
        public int DisplayOrder { get; set; }
        public TimeSpan? TimeLimit { get; set; }

        // Navigation properties
        public int QuizId { get; set; }
        public virtual Quiz Quiz { get; set; } = null!;
        public virtual ICollection<Option> Options { get; set; } = new HashSet<Option>();
        public virtual ICollection<QuestionResponse> Responses { get; set; } = new HashSet<QuestionResponse>();
        public virtual QuestionMetadata Metadata { get; set; } = null!;

        // Validation
        public bool IsValid() =>
            !string.IsNullOrWhiteSpace(Text) &&
            Points >= 0 &&
            (Type != QuestionType.MultipleChoice || Options.Any());
    }
}
