using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizApplication.DAL.Common
{
    public record AccessControl
    {
        private HashSet<string> _allowedUserIds = new();
        private HashSet<string> _allowedRoles = new();

        public bool IsPublic { get; init; }
        public string? AccessCode { get; init; }
        public IReadOnlySet<string> AllowedUserIds => _allowedUserIds;
        public IReadOnlySet<string> AllowedRoles => _allowedRoles;
        public DateTimeOffset? ExpiryDate { get; init; }

        public bool HasAccess(string userId, IEnumerable<string>? userRoles = null)
        {
            if (IsPublic) return true;
            if (ExpiryDate.HasValue && ExpiryDate.Value < DateTimeOffset.UtcNow) return false;

            return _allowedUserIds.Contains(userId) ||
                   (userRoles?.Any(role => _allowedRoles.Contains(role)) ?? false);
        }

        public void AddAllowedUser(string userId) => _allowedUserIds.Add(userId);
        public void AddAllowedRole(string role) => _allowedRoles.Add(role);
    }
}
