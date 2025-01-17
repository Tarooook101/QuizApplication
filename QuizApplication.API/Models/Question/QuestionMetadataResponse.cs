namespace QuizApplication.API.Models.Question
{
    public class QuestionMetadataResponse
    {
        public int Id { get; set; }
        public string? SourceReference { get; set; }
        public string? TopicArea { get; set; }
        public string? LearningObjective { get; set; }
        public Dictionary<string, string> CustomMetadata { get; set; } = new();

        public static QuestionMetadataResponse FromEntity(DAL.Entities.QuestionMetadata entity)
        {
            return new QuestionMetadataResponse
            {
                Id = entity.Id,
                SourceReference = entity.SourceReference,
                TopicArea = entity.TopicArea,
                LearningObjective = entity.LearningObjective,
                CustomMetadata = new Dictionary<string, string>(entity.CustomMetadata)
            };
        }
    }
}
