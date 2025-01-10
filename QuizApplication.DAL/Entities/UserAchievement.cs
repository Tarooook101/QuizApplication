using QuizApplication.DAL.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizApplication.DAL.Entities
{
    public class UserAchievement : BaseEntity<int>
    {
        public required string UserId { get; set; }
        public required int AchievementId { get; set; }
        public DateTimeOffset AwardedDate { get; set; }
        public int? Score { get; set; }

        public virtual ApplicationUser User { get; set; } = null!;
        public virtual Achievement Achievement { get; set; } = null!;
    }
}
