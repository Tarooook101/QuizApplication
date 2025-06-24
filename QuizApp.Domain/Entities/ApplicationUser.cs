using Microsoft.AspNetCore.Identity;

namespace QuizApp.Domain.Entities;

public class ApplicationUser : IdentityUser<Guid>
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? ProfilePictureUrl { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime? LastLoginAt { get; set; }

    // Navigation properties for quiz-related entities
    // public ICollection<Quiz> CreatedQuizzes { get; set; } = new List<Quiz>();
    // public ICollection<QuizAttempt> QuizAttempts { get; set; } = new List<QuizAttempt>();

    public string FullName => $"{FirstName} {LastName}".Trim();

    public ApplicationUser()
    {
        Id = Guid.NewGuid();
        CreatedAt = DateTime.UtcNow;
    }

    public void UpdateProfile(string firstName, string lastName, string? profilePictureUrl = null)
    {
        FirstName = firstName;
        LastName = lastName;
        if (profilePictureUrl != null)
        {
            ProfilePictureUrl = profilePictureUrl;
        }
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateLastLogin()
    {
        LastLoginAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Deactivate()
    {
        IsActive = false;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Activate()
    {
        IsActive = true;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateSecurityStamp()
    {
        SecurityStamp = Guid.NewGuid().ToString();
        UpdatedAt = DateTime.UtcNow;
    }
}