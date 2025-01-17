using System.ComponentModel.DataAnnotations;

namespace QuizApplication.API.Models.Option
{
    public class CreateOptionRequest
    {
        [Required]
        [StringLength(500)]
        public string Text { get; set; } = null!;

        public bool IsCorrect { get; set; }

        public int DisplayOrder { get; set; }

        [StringLength(1000)]
        public string? Explanation { get; set; }

        public DAL.Entities.Option ToEntity(DAL.Entities.Question question)
        {
            return new DAL.Entities.Option
            {
                Text = Text,
                IsCorrect = IsCorrect,
                DisplayOrder = DisplayOrder,
                Explanation = Explanation,
                Question = question,
                QuestionId = question.Id
            };
        }
    }
}
