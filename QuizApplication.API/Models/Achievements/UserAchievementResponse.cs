using QuizApplication.DAL.Entities;

namespace QuizApplication.API.Models.Achievements
{
    public record UserAchievementResponse
    {
        public int Id { get; init; }
        public AchievementResponse Achievement { get; init; } = null!;
        public DateTimeOffset AwardedDate { get; init; }
        public int? Score { get; init; }

        public static UserAchievementResponse FromEntity(UserAchievement userAchievement) => new()
        {
            Id = userAchievement.Id,
            Achievement = AchievementResponse.FromEntity(userAchievement.Achievement),
            AwardedDate = userAchievement.AwardedDate,
            Score = userAchievement.Score
        };
    }
}
