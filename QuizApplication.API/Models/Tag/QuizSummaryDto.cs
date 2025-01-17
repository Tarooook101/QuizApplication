using QuizApplication.DAL.Common;

namespace QuizApplication.API.Models.Tag
{
    public class QuizSummaryDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public string Description { get; set; } = null!;
        public QuizDifficulty Difficulty { get; set; }
        public QuizStatus Status { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
    }
}
