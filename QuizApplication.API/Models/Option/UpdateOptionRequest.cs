using System.ComponentModel.DataAnnotations;

namespace QuizApplication.API.Models.Option
{
    public class UpdateOptionRequest
    {
        public int Id { get; set; }

        [Required]
        [StringLength(500)]
        public string Text { get; set; } = null!;

        public bool IsCorrect { get; set; }

        public int DisplayOrder { get; set; }

        [StringLength(1000)]
        public string? Explanation { get; set; }

        public DAL.Entities.Option ToEntity(int questionId)
        {
            return new DAL.Entities.Option
            {
                Id = Id,
                QuestionId = questionId,
                Text = Text,
                IsCorrect = IsCorrect,
                DisplayOrder = DisplayOrder,
                Explanation = Explanation
            };
        }
    }
}
