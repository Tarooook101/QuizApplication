using QuizApplication.API.Models.Option;
using QuizApplication.DAL.Entities;
using QuizApplication.API.Models.Question;

namespace QuizApplication.API.Models.Question
{
    public class QuestionDetailResponse : QuestionResponse
    {
        public string? Explanation { get; set; }
        public bool IsMandatory { get; set; }
        public TimeSpan? TimeLimit { get; set; }
        public List<OptionResponse> Options { get; set; } = new();
        public QuestionMetadataResponse? Metadata { get; set; }

        public new static QuestionDetailResponse FromEntity(DAL.Entities.Question entity)
        {
            return new QuestionDetailResponse
            {
                Id = entity.Id,
                Text = entity.Text,
                Points = entity.Points,
                Type = entity.Type,
                Difficulty = entity.Difficulty,
                ImageUrl = entity.ImageUrl,
                DisplayOrder = entity.DisplayOrder,
                Explanation = entity.Explanation,
                IsMandatory = entity.IsMandatory,
                TimeLimit = entity.TimeLimit,
                Options = entity.Options.Select(OptionResponse.FromEntity).ToList(),
                Metadata = entity.Metadata != null ? QuestionMetadataResponse.FromEntity(entity.Metadata) : null
            };
        }
    }
}
