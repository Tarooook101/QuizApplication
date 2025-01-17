using QuizApplication.DAL.Entities;
using System.ComponentModel.DataAnnotations;

namespace QuizApplication.API.Models.Question
{
    public class CreateQuestionMetadataRequest
    {
        [StringLength(500)]
        public string? SourceReference { get; set; }

        [StringLength(200)]
        public string? TopicArea { get; set; }

        [StringLength(500)]
        public string? LearningObjective { get; set; }

        public Dictionary<string, string> CustomMetadata { get; set; } = new();

        public QuestionMetadata ToEntity(int questionId)
        {
            return new QuestionMetadata
            {
                QuestionId = questionId,  
                SourceReference = SourceReference,
                TopicArea = TopicArea,
                LearningObjective = LearningObjective,
                CustomMetadata = new Dictionary<string, string>(CustomMetadata)
            };
        }
    }
}
