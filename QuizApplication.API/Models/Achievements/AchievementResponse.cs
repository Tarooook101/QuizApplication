using QuizApplication.DAL.Common;
using QuizApplication.DAL.Entities;

namespace QuizApplication.API.Models.Achievements
{
    public record AchievementResponse
    {
        public int Id { get; init; }
        public string Name { get; init; } = null!;
        public string Description { get; init; } = null!;
        public string IconUrl { get; init; } = null!;
        public int RequiredPoints { get; init; }
        public AchievementType Type { get; init; }
        public Dictionary<string, string> Criteria { get; init; } = new();
        public DateTimeOffset CreatedAt { get; init; }
        public DateTimeOffset? LastModifiedAt { get; init; }

        public static AchievementResponse FromEntity(Achievement achievement) => new()
        {
            Id = achievement.Id,
            Name = achievement.Name,
            Description = achievement.Description,
            IconUrl = achievement.IconUrl,
            RequiredPoints = achievement.RequiredPoints,
            Type = achievement.Type,
            Criteria = new Dictionary<string, string>(achievement.Criteria),
            CreatedAt = achievement.CreatedAt,
            LastModifiedAt = achievement.LastModifiedAt
        };
    }
}
