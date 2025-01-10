using QuizApplication.DAL.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizApplication.DAL.Entities
{
    public class Achievement : BaseEntity<int>
    {
        public required string Name { get; set; }
        public required string Description { get; set; }
        public required string IconUrl { get; set; }
        public int RequiredPoints { get; set; }
        public AchievementType Type { get; set; }
        public Dictionary<string, string> Criteria { get; set; } = new();

        public virtual ICollection<UserAchievement> UserAchievements { get; set; } = new HashSet<UserAchievement>();

        public bool IsEarned(int userPoints) => userPoints >= RequiredPoints;
    }
}
