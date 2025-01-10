

using QuizApplication.DAL.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizApplication.DAL.Entities
{
    public class QuestionMetadata : BaseEntity<int>
    {
        public required int QuestionId { get; set; }
        public string? SourceReference { get; set; }
        public string? TopicArea { get; set; }
        public string? LearningObjective { get; set; }

        private Dictionary<string, string> _customMetadata = new();
        public Dictionary<string, string> CustomMetadata
        {
            get => new(_customMetadata);
            set => _customMetadata = value ?? new Dictionary<string, string>();
        }

        public virtual Question Question { get; set; } = null!;
    }
}
