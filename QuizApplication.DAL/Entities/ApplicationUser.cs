using Microsoft.AspNetCore.Identity;
using QuizApplication.DAL.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizApplication.DAL.Entities
{
    public class ApplicationUser : IdentityUser, IAuditableEntity, ISoftDeletable
    {
        public required string FirstName { get; set; }  // Use required keyword
        public required string LastName { get; set; }
        public string? ProfilePictureUrl { get; set; }
        public UserRole Role { get; set; }
        public UserStatus Status { get; set; }
        public string? RefreshToken { get; set; }
        public DateTimeOffset? RefreshTokenExpiryTime { get; set; }

        // Computed property for full name
        public string FullName => $"{FirstName} {LastName}";

        // Audit properties
        public string CreatedBy { get; set; } = null!;
        public DateTimeOffset CreatedAt { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTimeOffset? LastModifiedAt { get; set; }
        public bool IsDeleted { get; set; }
        public string? DeletedBy { get; set; }
        public DateTimeOffset? DeletedAt { get; set; }

        // Navigation properties
        public virtual ICollection<Quiz> CreatedQuizzes { get; set; } = new HashSet<Quiz>();  // Initialize collections
        public virtual ICollection<QuizAttempt> QuizAttempts { get; set; } = new HashSet<QuizAttempt>();
        public virtual ICollection<UserAchievement> Achievements { get; set; } = new HashSet<UserAchievement>();
        public virtual UserProfile Profile { get; set; } = null!;
    }
}
