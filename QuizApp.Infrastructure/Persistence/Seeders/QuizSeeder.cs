using Microsoft.EntityFrameworkCore;
using QuizApp.Domain.Entities;
using QuizApp.Domain.Enums;


namespace QuizApp.Infrastructure.Persistence.Seeders;

public static class QuizSeeder
{
    public static async Task SeedAsync(QuizDbContext context)
    {
        if (await context.Set<Quiz>().AnyAsync())
        {
            return; // Data already seeded
        }

        // Get the first user (assuming users exist from category seeding or manual creation)
        var firstUser = await context.Users.FirstOrDefaultAsync();
        if (firstUser == null)
        {
            // Create a default system user if no users exist
            firstUser = new ApplicationUser
            {
                Id = Guid.NewGuid(),
                UserName = "system@quizapp.com",
                NormalizedUserName = "SYSTEM@QUIZAPP.COM",
                Email = "system@quizapp.com",
                NormalizedEmail = "SYSTEM@QUIZAPP.COM",
                EmailConfirmed = true,
                FirstName = "System",
                LastName = "Admin",
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            await context.Users.AddAsync(firstUser);
            await context.SaveChangesAsync();
        }

        var quizzes = new List<Quiz>
        {
            new Quiz(
                title: "Basic Programming Concepts",
                description: "Test your knowledge of fundamental programming concepts including variables, loops, and functions.",
                timeLimit: 30,
                maxAttempts: 3,
                difficulty: QuizDifficulty.Beginner,
                createdByUserId: firstUser.Id,
                instructions: "Answer all questions to the best of your ability. You have 30 minutes to complete this quiz.",
                isPublic: true,
                thumbnailUrl: null,
                tags: "programming,basics,beginner"
            ),

            new Quiz(
                title: "Advanced C# Features",
                description: "Explore advanced C# concepts including LINQ, async/await, generics, and design patterns.",
                timeLimit: 45,
                maxAttempts: 2,
                difficulty: QuizDifficulty.Advanced,
                createdByUserId: firstUser.Id,
                instructions: "This quiz covers advanced C# topics. Make sure you understand delegates, events, and async programming.",
                isPublic: true,
                thumbnailUrl: null,
                tags: "csharp,advanced,linq,async"
            ),

            new Quiz(
                title: "Database Design Principles",
                description: "Test your understanding of database normalization, relationships, and SQL optimization.",
                timeLimit: 40,
                maxAttempts: 3,
                difficulty: QuizDifficulty.Intermediate,
                createdByUserId: firstUser.Id,
                instructions: "Focus on database design best practices and SQL query optimization techniques.",
                isPublic: true,
                thumbnailUrl: null,
                tags: "database,sql,design,normalization"
            ),

            new Quiz(
                title: "Web API Security",
                description: "Learn about authentication, authorization, HTTPS, and common security vulnerabilities in web APIs.",
                timeLimit: 35,
                maxAttempts: 2,
                difficulty: QuizDifficulty.Intermediate,
                createdByUserId: firstUser.Id,
                instructions: "This quiz covers JWT tokens, OAuth, CORS, and API security best practices.",
                isPublic: true,
                thumbnailUrl: null,
                tags: "security,api,authentication,jwt"
            ),

            new Quiz(
                title: "Machine Learning Fundamentals",
                description: "Introduction to machine learning algorithms, supervised vs unsupervised learning, and model evaluation.",
                timeLimit: 50,
                maxAttempts: 3,
                difficulty: QuizDifficulty.Expert,
                createdByUserId: firstUser.Id,
                instructions: "This comprehensive quiz covers ML algorithms, data preprocessing, and model validation techniques.",
                isPublic: true,
                thumbnailUrl: null,
                tags: "machine-learning,ai,algorithms,data-science"
            )
        };

        await context.Set<Quiz>().AddRangeAsync(quizzes);
        await context.SaveChangesAsync();
    }
}