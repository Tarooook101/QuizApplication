using QuizApplication.DAL.Common;
using QuizApplication.DAL.Entities;

namespace QuizApplication.API.Models.Achievements
{
    public record CreateAchievementRequest
    {
        public required string Name { get; init; }
        public required string Description { get; init; }
        public required string IconUrl { get; init; }
        public required int RequiredPoints { get; init; }
        public required AchievementType Type { get; init; }
        public Dictionary<string, string> Criteria { get; init; } = new();

        public Achievement ToEntity() => new()
        {
            Name = Name,
            Description = Description,
            IconUrl = IconUrl,
            RequiredPoints = RequiredPoints,
            Type = Type,
            Criteria = new Dictionary<string, string>(Criteria)
        };
    }
}
