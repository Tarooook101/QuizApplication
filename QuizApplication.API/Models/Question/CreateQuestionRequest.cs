using QuizApplication.API.Models.Option;
using QuizApplication.DAL.Common;
using QuizApplication.DAL.Entities;
using System.ComponentModel.DataAnnotations;

namespace QuizApplication.API.Models.Question
{
    public class CreateQuestionRequest
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

        public List<CreateOptionRequest> Options { get; set; } = new();

        public CreateQuestionMetadataRequest? Metadata { get; set; }

        public DAL.Entities.Question ToEntity()
        {
            var question = new DAL.Entities.Question
            {
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

            // Add options if present
            if (Options.Any())
            {
                question.Options = Options.Select(o => o.ToEntity(question)).ToList();
            }

            // Add metadata if present
            if (Metadata != null)
            {
                question.Metadata = new QuestionMetadata
                {
                    QuestionId = default, // This will be set by EF Core when the question is saved
                    Question = question,  // Set the navigation property
                    SourceReference = Metadata.SourceReference,
                    TopicArea = Metadata.TopicArea,
                    LearningObjective = Metadata.LearningObjective,
                    CustomMetadata = new Dictionary<string, string>(Metadata.CustomMetadata)
                };
            }

            return question;
        }
    }
}
