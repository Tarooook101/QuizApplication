using Microsoft.EntityFrameworkCore;
using QuizApp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizApp.Infrastructure.Persistence.Seeders;

public static class QuizReviewSeeder
{
    public static async Task SeedAsync(QuizDbContext context)
    {
        if (await context.Set<QuizReview>().AnyAsync())
        {
            return; // Data already seeded
        }

        // Get existing quizzes and users
        var quizzes = await context.Set<Quiz>().Take(5).ToListAsync();
        var users = await context.Users.Take(10).ToListAsync();

        if (!quizzes.Any() || !users.Any())
        {
            return; // No quizzes or users exist to create reviews for
        }

        var random = new Random();
        var quizReviews = new List<QuizReview>();

        // Create reviews for each quiz
        foreach (var quiz in quizzes)
        {
            // Generate 2-4 reviews per quiz
            var reviewCount = random.Next(2, 5);
            var usersForThisQuiz = users.OrderBy(x => random.Next()).Take(reviewCount).ToList();

            foreach (var user in usersForThisQuiz)
            {
                // Generate realistic rating (more likely to be 3-5)
                var rating = GenerateRealisticRating(random);
                var comment = GenerateReviewComment(quiz.Title, rating, random);
                var isRecommended = rating >= 3;
                var isPublic = random.Next(1, 101) <= 85; // 85% chance to be public

                var review = new QuizReview(
                    quizId: quiz.Id,
                    userId: user.Id,
                    rating: rating,
                    comment: comment,
                    isRecommended: isRecommended,
                    isPublic: isPublic
                );

                quizReviews.Add(review);
            }
        }

        await context.Set<QuizReview>().AddRangeAsync(quizReviews);
        await context.SaveChangesAsync();
    }

    private static int GenerateRealisticRating(Random random)
    {
        // Generate more realistic ratings (weighted towards higher ratings)
        var weights = new[] { 5, 10, 25, 35, 25 }; // 1-star: 5%, 2-star: 10%, 3-star: 25%, 4-star: 35%, 5-star: 25%
        var totalWeight = weights.Sum();
        var randomValue = random.Next(1, totalWeight + 1);

        var cumulativeWeight = 0;
        for (int i = 0; i < weights.Length; i++)
        {
            cumulativeWeight += weights[i];
            if (randomValue <= cumulativeWeight)
            {
                return i + 1; // Return 1-5 stars
            }
        }

        return 4; // Default fallback
    }

    private static string? GenerateReviewComment(string quizTitle, int rating, Random random)
    {
        // 20% chance of no comment
        if (random.Next(1, 101) <= 20)
        {
            return null;
        }

        var positiveComments = new[]
        {
            $"Excellent quiz on {quizTitle.ToLower()}! Really helped me understand the concepts better.",
            "Great questions and well-structured content. Highly recommended for anyone learning this topic.",
            "Perfect difficulty level and very informative. Will definitely take more quizzes from this creator.",
            "Loved the practical examples and clear explanations. This quiz is a gem!",
            "Outstanding content! The questions were challenging but fair.",
            "Very comprehensive coverage of the topic. Great for both beginners and intermediate learners.",
            "Brilliant quiz design! The questions flow logically and build upon each other.",
            "Fantastic resource for learning. The explanations are clear and helpful."
        };

        var neutralComments = new[]
        {
            "Decent quiz overall. Some questions could be clearer but generally good content.",
            "Good quiz for the basics. Could use more advanced questions for experienced learners.",
            "Average quiz. Covers the fundamentals well but nothing particularly outstanding.",
            "Solid content. The time limit is reasonable and questions are fair.",
            "Not bad for a quick review of the concepts. Would be nice to have more detailed explanations.",
            "Okay quiz. It serves its purpose but could be more engaging.",
            "Standard quiz content. Gets the job done for basic understanding."
        };

        var negativeComments = new[]
        {
            "The quiz has some good points but several questions are ambiguous or poorly worded.",
            "Expected more depth in the questions. Some topics are covered too superficially.",
            "Not great. Some questions seem outdated and don't reflect current best practices.",
            "Disappointing. The quiz doesn't match the description and difficulty level claimed.",
            "Could be much better. The questions lack clarity and some answers seem questionable.",
            "Below average. Expected more comprehensive coverage of the topic.",
            "Needs improvement. Some questions are confusing and the flow could be better."
        };

        return rating switch
        {
            5 or 4 => positiveComments[random.Next(positiveComments.Length)],
            3 => neutralComments[random.Next(neutralComments.Length)],
            _ => negativeComments[random.Next(negativeComments.Length)]
        };
    }
}