using Microsoft.EntityFrameworkCore;
using QuizApp.Domain.Entities;
using QuizApp.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizApp.Infrastructure.Persistence.Seeders;

public static class QuizAttemptSeeder
{
    public static async Task SeedAsync(QuizDbContext context)
    {
        if (await context.Set<QuizAttempt>().AnyAsync())
        {
            return; // Data already seeded
        }

        // Get existing users and quizzes
        var users = await context.Users.Take(3).ToListAsync();
        var quizzes = await context.Set<Quiz>().Take(5).ToListAsync();

        if (!users.Any() || !quizzes.Any())
        {
            return; // No users or quizzes to create attempts for
        }

        var quizAttempts = new List<QuizAttempt>();
        var random = new Random();

        // Create various quiz attempts with different statuses and scenarios
        foreach (var user in users)
        {
            foreach (var quiz in quizzes.Take(3)) // Each user attempts first 3 quizzes
            {
                // Create completed attempts
                var completedAttempt = new QuizAttempt(quiz.Id, user.Id);

                // Simulate the attempt was started some time ago
                var startTime = DateTime.UtcNow.AddDays(-random.Next(1, 30)).AddHours(-random.Next(0, 23));
                typeof(QuizAttempt).GetProperty("StartedAt")?.SetValue(completedAttempt, startTime);

                // Complete the attempt with random scores
                var maxScore = random.Next(50, 100);
                var score = random.Next(20, maxScore);
                var notes = GetRandomNotes(random);

                completedAttempt.Complete(score, maxScore, notes);

                quizAttempts.Add(completedAttempt);

                // 30% chance to create a second attempt for the same quiz
                if (random.NextDouble() < 0.3)
                {
                    var secondAttempt = new QuizAttempt(quiz.Id, user.Id);
                    var secondStartTime = DateTime.UtcNow.AddDays(-random.Next(1, 15)).AddHours(-random.Next(0, 23));
                    typeof(QuizAttempt).GetProperty("StartedAt")?.SetValue(secondAttempt, secondStartTime);

                    var secondMaxScore = random.Next(50, 100);
                    var secondScore = random.Next(30, secondMaxScore);
                    var secondNotes = GetRandomNotes(random);

                    secondAttempt.Complete(secondScore, secondMaxScore, secondNotes);
                    quizAttempts.Add(secondAttempt);
                }
            }

            // Create some in-progress attempts (10% chance per remaining quiz)
            foreach (var quiz in quizzes.Skip(3))
            {
                if (random.NextDouble() < 0.1)
                {
                    var inProgressAttempt = new QuizAttempt(quiz.Id, user.Id);
                    var startTime = DateTime.UtcNow.AddHours(-random.Next(0, 4)); // Started within last 4 hours
                    typeof(QuizAttempt).GetProperty("StartedAt")?.SetValue(inProgressAttempt, startTime);

                    // Optionally add some progress notes
                    if (random.NextDouble() < 0.5)
                    {
                        inProgressAttempt.UpdateProgress("Working through the questions...");
                    }

                    quizAttempts.Add(inProgressAttempt);
                }
            }

            // Create some abandoned attempts (5% chance)
            if (random.NextDouble() < 0.05)
            {
                var abandonedQuiz = quizzes[random.Next(quizzes.Count)];
                var abandonedAttempt = new QuizAttempt(abandonedQuiz.Id, user.Id);
                var startTime = DateTime.UtcNow.AddDays(-random.Next(1, 7)).AddHours(-random.Next(0, 23));
                typeof(QuizAttempt).GetProperty("StartedAt")?.SetValue(abandonedAttempt, startTime);

                abandonedAttempt.Abandon();
                quizAttempts.Add(abandonedAttempt);
            }
        }

        // Add some high-performing attempts
        var topUser = users.First();
        var expertQuiz = quizzes.FirstOrDefault(q => q.Difficulty == QuizDifficulty.Expert);
        if (expertQuiz != null)
        {
            var expertAttempt = new QuizAttempt(expertQuiz.Id, topUser.Id);
            var expertStartTime = DateTime.UtcNow.AddDays(-random.Next(1, 10));
            typeof(QuizAttempt).GetProperty("StartedAt")?.SetValue(expertAttempt, expertStartTime);

            expertAttempt.Complete(95, 100, "Excellent performance on expert level quiz!");
            quizAttempts.Add(expertAttempt);
        }

        // Add some poor-performing attempts to show variety
        var beginnerQuiz = quizzes.FirstOrDefault(q => q.Difficulty == QuizDifficulty.Beginner);
        if (beginnerQuiz != null && users.Count > 1)
        {
            var strugglingUser = users[1];
            var poorAttempt = new QuizAttempt(beginnerQuiz.Id, strugglingUser.Id);
            var poorStartTime = DateTime.UtcNow.AddDays(-random.Next(1, 5));
            typeof(QuizAttempt).GetProperty("StartedAt")?.SetValue(poorAttempt, poorStartTime);

            poorAttempt.Complete(15, 50, "Need to review the basics more thoroughly.");
            quizAttempts.Add(poorAttempt);
        }

        await context.Set<QuizAttempt>().AddRangeAsync(quizAttempts);
        await context.SaveChangesAsync();
    }

    private static string? GetRandomNotes(Random random)
    {
        var noteOptions = new[]
        {
            null, // No notes
            "Great quiz, learned a lot!",
            "Some questions were tricky but manageable.",
            "Need to review this topic more.",
            "Excellent content and clear questions.",
            "Found a few questions ambiguous.",
            "Perfect difficulty level for my skill.",
            "Challenging but fair assessment.",
            "Could use more detailed explanations.",
            "Very comprehensive coverage of the topic.",
            "Some questions were too easy, others too hard.",
            "Good practice for real-world scenarios."
        };

        return random.NextDouble() < 0.6 ? noteOptions[random.Next(noteOptions.Length)] : null;
    }
}