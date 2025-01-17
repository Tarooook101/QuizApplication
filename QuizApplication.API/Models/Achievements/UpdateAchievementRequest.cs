using QuizApplication.DAL.Common;
using QuizApplication.DAL.Entities;

namespace QuizApplication.API.Models.Achievements
{
    public record UpdateAchievementRequest
    {
        public required string Name { get; init; }
        public required string Description { get; init; }
        public required string IconUrl { get; init; }
        public required int RequiredPoints { get; init; }
        public required AchievementType Type { get; init; }
        public Dictionary<string, string> Criteria { get; init; } = new();

        public void UpdateEntity(Achievement achievement)
        {
            achievement.Name = Name;
            achievement.Description = Description;
            achievement.IconUrl = IconUrl;
            achievement.RequiredPoints = RequiredPoints;
            achievement.Type = Type;
            achievement.Criteria = new Dictionary<string, string>(Criteria);
        }
    }
}
