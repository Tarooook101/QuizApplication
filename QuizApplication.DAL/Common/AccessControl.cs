using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizApplication.DAL.Common
{
    public record AccessControl
    {
        public bool IsPublic { get; init; }
        public string? AccessCode { get; init; }
        private HashSet<string> AllowedUserIds { get; init; } = new();
        private HashSet<string> AllowedRoles { get; init; } = new();
        public DateTimeOffset? ExpiryDate { get; init; }

        public bool HasAccess(string userId, IEnumerable<string>? userRoles = null)
        {
            if (IsPublic) return true;
            if (ExpiryDate.HasValue && ExpiryDate.Value < DateTimeOffset.UtcNow) return false;

            return AllowedUserIds.Contains(userId) ||
                   (userRoles?.Any(role => AllowedRoles.Contains(role)) ?? false);
        }

        public void AddAllowedUser(string userId) => AllowedUserIds.Add(userId);
        public void AddAllowedRole(string role) => AllowedRoles.Add(role);
    }
}
