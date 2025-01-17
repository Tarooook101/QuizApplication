using QuizApplication.API.Models.Option;
using QuizApplication.DAL.Common;
using System.ComponentModel.DataAnnotations;

namespace QuizApplication.API.Models.Question
{
    public class UpdateQuestionRequest
    {
        [Required]
        [StringLength(1000)]
        public string Text { get; set; } = null!;

        [Range(0, int.MaxValue)]
        public int Points { get; set; }

        public QuestionType Type { get; set; }

        public QuestionDifficulty Difficulty { get; set; }

        [Url]
        public string? ImageUrl { get; set; }

        [StringLength(2000)]
        public string? Explanation { get; set; }

        public bool IsMandatory { get; set; }

        public TimeSpan? TimeLimit { get; set; }

        public int DisplayOrder { get; set; }

        public List<UpdateOptionRequest> Options { get; set; } = new();

        public UpdateQuestionMetadataRequest? Metadata { get; set; }

        public DAL.Entities.Question ToEntity(int id)
        {
            var question = new DAL.Entities.Question
            {
                Id = id,
                Text = Text,
                Points = Points,
                Type = Type,
                Difficulty = Difficulty,
                ImageUrl = ImageUrl,
                Explanation = Explanation,
                IsMandatory = IsMandatory,
                TimeLimit = TimeLimit,
                DisplayOrder = DisplayOrder
            };

            if (Options.Any())
            {
                question.Options = Options.Select(o => o.ToEntity(id)).ToList();
            }

            if (Metadata != null)
            {
                question.Metadata = Metadata.ToEntity(id);
            }

            return question;
        }
    }
}
