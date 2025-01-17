using QuizApplication.DAL.Common;
using QuizApplication.DAL.Entities;

namespace QuizApplication.API.Models.Question
{
    public class QuestionResponse
    {
        public int Id { get; set; }
        public string Text { get; set; } = null!;
        public int Points { get; set; }
        public QuestionType Type { get; set; }
        public QuestionDifficulty Difficulty { get; set; }
        public string? ImageUrl { get; set; }
        public int DisplayOrder { get; set; }

        public static QuestionResponse FromEntity(DAL.Entities.Question entity)
        {
            return new QuestionResponse
            {
                Id = entity.Id,
                Text = entity.Text,
                Points = entity.Points,
                Type = entity.Type,
                Difficulty = entity.Difficulty,
                ImageUrl = entity.ImageUrl,
                DisplayOrder = entity.DisplayOrder
            };
        }
    }
}
