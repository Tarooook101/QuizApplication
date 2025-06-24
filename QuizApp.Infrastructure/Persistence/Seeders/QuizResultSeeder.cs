using Microsoft.EntityFrameworkCore;
using QuizApp.Domain.Entities;
using QuizApp.Domain.Enums;


namespace QuizApp.Infrastructure.Persistence.Seeders;

public static class QuizResultSeeder
{
    public static async Task SeedAsync(QuizDbContext context)
    {
        if (await context.Set<QuizResult>().AnyAsync())
        {
            return; // Data already seeded
        }

        // Get completed quiz attempts that don't have results yet
        var completedAttempts = await context.Set<QuizAttempt>()
            .Where(qa => qa.Status == QuizAttemptStatus.Completed)
            .ToListAsync();

        if (!completedAttempts.Any())
        {
            return; // No completed attempts to create results for
        }

        // Preload quizzes for lookup
        var quizIds = completedAttempts.Select(a => a.QuizId).Distinct().ToList();
        var quizzes = await context.Set<Quiz>()
            .Where(q => quizIds.Contains(q.Id))
            .ToListAsync();
        var quizLookup = quizzes.ToDictionary(q => q.Id, q => q);

        var quizResults = new List<QuizResult>();
        var random = new Random();

        foreach (var attempt in completedAttempts)
        {
            // Lookup quiz by QuizId
            quizLookup.TryGetValue(attempt.QuizId, out var quiz);
            var difficulty = quiz?.Difficulty ?? QuizDifficulty.Beginner;

            // Generate realistic data based on the attempt
            var totalQuestions = GetTotalQuestionsForQuiz(difficulty, random);
            var correctAnswers = CalculateCorrectAnswers(attempt.Score ?? 0, attempt.MaxScore ?? 100, totalQuestions, random);
            var timeSpent = CalculateTimeSpent(attempt.StartedAt, attempt.CompletedAt ?? DateTime.UtcNow, random);
            var passingThreshold = GetPassingThreshold(difficulty);

            var quizResult = new QuizResult(
                quizAttemptId: attempt.Id,
                userId: attempt.UserId,
                quizId: attempt.QuizId,
                score: attempt.Score ?? 0,
                maxScore: attempt.MaxScore ?? 100,
                correctAnswers: correctAnswers,
                totalQuestions: totalQuestions,
                timeSpent: timeSpent,
                passingThreshold: passingThreshold
            );

            // Add feedback based on performance
            var feedback = GenerateFeedback(quizResult, difficulty, random);
            if (!string.IsNullOrEmpty(feedback))
            {
                quizResult.UpdateFeedback(feedback);
            }

            quizResults.Add(quizResult);
        }

        // Create some exceptional results for demonstration
        await CreateExceptionalResults(context, quizResults, random);

        await context.Set<QuizResult>().AddRangeAsync(quizResults);
        await context.SaveChangesAsync();
    }

    private static int GetTotalQuestionsForQuiz(QuizDifficulty difficulty, Random random)
    {
        return difficulty switch
        {
            QuizDifficulty.Beginner => random.Next(10, 16), // 10-15 questions
            QuizDifficulty.Intermediate => random.Next(15, 21), // 15-20 questions
            QuizDifficulty.Advanced => random.Next(20, 26), // 20-25 questions
            QuizDifficulty.Expert => random.Next(25, 31), // 25-30 questions
            _ => random.Next(10, 16)
        };
    }

    private static int CalculateCorrectAnswers(int score, int maxScore, int totalQuestions, Random random)
    {
        // Calculate based on score percentage with some realistic variance
        var percentage = (double)score / maxScore;
        var baseCorrect = (int)Math.Round(totalQuestions * percentage);

        // Add small variance (±1-2 questions) to make it more realistic
        var variance = random.Next(-2, 3);
        var correctAnswers = Math.Max(0, Math.Min(totalQuestions, baseCorrect + variance));

        return correctAnswers;
    }

    private static TimeSpan CalculateTimeSpent(DateTime startedAt, DateTime completedAt, Random random)
    {
        var actualDuration = completedAt - startedAt;

        // If the actual duration seems too long (more than 3 hours), generate a realistic time
        if (actualDuration.TotalHours > 3)
        {
            var minutes = random.Next(15, 120); // 15 minutes to 2 hours
            return TimeSpan.FromMinutes(minutes);
        }

        return actualDuration;
    }

    private static double GetPassingThreshold(QuizDifficulty difficulty)
    {
        return difficulty switch
        {
            QuizDifficulty.Beginner => 60.0,
            QuizDifficulty.Intermediate => 65.0,
            QuizDifficulty.Advanced => 70.0,
            QuizDifficulty.Expert => 75.0,
            _ => 60.0
        };
    }

    private static string? GenerateFeedback(QuizResult result, QuizDifficulty difficulty, Random random)
    {
        var feedbackTemplates = GetFeedbackTemplates(result.IsPassed, difficulty);

        if (feedbackTemplates.Any() && random.NextDouble() < 0.7) // 70% chance of having feedback
        {
            var template = feedbackTemplates[random.Next(feedbackTemplates.Count)];
            return string.Format(template, result.Percentage, result.CorrectAnswers, result.TotalQuestions);
        }

        return null;
    }

    private static List<string> GetFeedbackTemplates(bool passed, QuizDifficulty difficulty)
    {
        if (passed)
        {
            return difficulty switch
            {
                QuizDifficulty.Beginner => new List<string>
                {
                    "Great job! You scored {0:F1}% and got {1} out of {2} questions correct. You have a solid foundation in the basics.",
                    "Well done! Your score of {0:F1}% shows you understand the fundamental concepts. Keep up the good work!",
                    "Excellent work! You answered {1} out of {2} questions correctly. You're ready to move to intermediate topics."
                },
                QuizDifficulty.Intermediate => new List<string>
                {
                    "Impressive! Your {0:F1}% score demonstrates strong intermediate knowledge. You got {1} out of {2} questions right.",
                    "Great performance! Scoring {0:F1}% on this intermediate quiz shows you're developing solid expertise.",
                    "Well done! Your {1} correct answers out of {2} questions show good progress in advanced concepts."
                },
                QuizDifficulty.Advanced => new List<string>
                {
                    "Outstanding! A {0:F1}% score on this advanced quiz is truly impressive. You answered {1} out of {2} questions correctly.",
                    "Excellent work! Your {0:F1}% score demonstrates advanced understanding. You're clearly an expert in this area.",
                    "Superb performance! Getting {1} out of {2} questions correct on this challenging quiz shows exceptional knowledge."
                },
                QuizDifficulty.Expert => new List<string>
                {
                    "Exceptional! Scoring {0:F1}% on this expert-level quiz is remarkable. You got {1} out of {2} questions right.",
                    "Outstanding mastery! Your {0:F1}% score places you among the top performers. Excellent work on {1} out of {2} questions.",
                    "Phenomenal! This {0:F1}% score on our most challenging quiz demonstrates true expertise. {1} correct out of {2} is impressive."
                },
                _ => new List<string>()
            };
        }
        else
        {
            return difficulty switch
            {
                QuizDifficulty.Beginner => new List<string>
                {
                    "You scored {0:F1}% with {1} out of {2} questions correct. Review the basics and try again - you can do it!",
                    "Your {0:F1}% score shows you need more practice with fundamentals. Focus on the areas you missed.",
                    "With {1} out of {2} correct answers, you're on the right track. Review the material and retake when ready."
                },
                QuizDifficulty.Intermediate => new List<string>
                {
                    "You scored {0:F1}% on this intermediate quiz. Getting {1} out of {2} correct shows potential - keep studying!",
                    "Your {0:F1}% score indicates you need more preparation for intermediate concepts. Review and try again.",
                    "With {1} correct answers out of {2}, you're making progress. Focus on the challenging areas and retake."
                },
                QuizDifficulty.Advanced => new List<string>
                {
                    "Scoring {0:F1}% on this advanced quiz shows you need more preparation. You got {1} out of {2} questions right.",
                    "Your {0:F1}% score indicates this advanced material needs more study. Review the complex topics thoroughly.",
                    "Getting {1} out of {2} correct on this challenging quiz is a good start. Continue studying the advanced concepts."
                },
                QuizDifficulty.Expert => new List<string>
                {
                    "This expert quiz is very challenging - your {0:F1}% score with {1} out of {2} correct shows good effort.",
                    "Expert level content is tough! Your {0:F1}% score shows you're learning. Keep studying the advanced materials.",
                    "Getting {1} out of {2} questions correct on this expert quiz is respectable. Continue building your expertise."
                },
                _ => new List<string>()
            };
        }
    }

    private static async Task CreateExceptionalResults(QuizDbContext context, List<QuizResult> existingResults, Random random)
    {
        // Get some additional attempts to create special scenarios
        var users = await context.Users.Take(2).ToListAsync();
        var quizzes = await context.Set<Quiz>().Take(3).ToListAsync();

        if (!users.Any() || !quizzes.Any()) return;

        // Create a perfect score result
        var expertQuiz = quizzes.FirstOrDefault(q => q.Difficulty == QuizDifficulty.Expert);
        if (expertQuiz != null && users.Any())
        {
            var perfectAttempt = new QuizAttempt(expertQuiz.Id, users[0].Id);

            // Set completed status and perfect score
            var perfectQuestions = 30;
            var perfectTime = TimeSpan.FromMinutes(random.Next(35, 50));

            perfectAttempt.Complete(100, 100, "Perfect execution!");

            var perfectResult = new QuizResult(
                quizAttemptId: perfectAttempt.Id,
                userId: perfectAttempt.UserId,
                quizId: perfectAttempt.QuizId,
                score: 100,
                maxScore: 100,
                correctAnswers: perfectQuestions,
                totalQuestions: perfectQuestions,
                timeSpent: perfectTime,
                passingThreshold: 75.0
            );

            perfectResult.UpdateFeedback("Perfect score! You answered all 30 questions correctly in excellent time. Exceptional mastery of expert-level content!");

            // Add the attempt first, then the result
            await context.Set<QuizAttempt>().AddAsync(perfectAttempt);
            existingResults.Add(perfectResult);
        }

        // Create a barely passing result
        var intermediateQuiz = quizzes.FirstOrDefault(q => q.Difficulty == QuizDifficulty.Intermediate);
        if (intermediateQuiz != null && users.Count > 1)
        {
            var barelyPassingAttempt = new QuizAttempt(intermediateQuiz.Id, users[1].Id);

            var totalQuestions = 18;
            var correctAnswers = 12; // Just above 65% threshold
            var score = 66;
            var timeSpent = TimeSpan.FromMinutes(random.Next(25, 35));

            barelyPassingAttempt.Complete(score, 100, "Just made it!");

            var barelyPassingResult = new QuizResult(
                quizAttemptId: barelyPassingAttempt.Id,
                userId: barelyPassingAttempt.UserId,
                quizId: barelyPassingAttempt.QuizId,
                score: score,
                maxScore: 100,
                correctAnswers: correctAnswers,
                totalQuestions: totalQuestions,
                timeSpent: timeSpent,
                passingThreshold: 65.0
            );

            barelyPassingResult.UpdateFeedback("You passed with 66.0%! Getting 12 out of 18 questions correct meets the requirements, but consider reviewing to strengthen your knowledge.");

            await context.Set<QuizAttempt>().AddAsync(barelyPassingAttempt);
            existingResults.Add(barelyPassingResult);
        }

        // Create a high-speed completion result
        var beginnerQuiz = quizzes.FirstOrDefault(q => q.Difficulty == QuizDifficulty.Beginner);
        if (beginnerQuiz != null && users.Any())
        {
            var speedAttempt = new QuizAttempt(beginnerQuiz.Id, users[0].Id);

            var questions = 12;
            var correct = 11;
            var score = 92;
            var fastTime = TimeSpan.FromMinutes(8); // Very fast completion

            speedAttempt.Complete(score, 100, "Lightning fast!");

            var speedResult = new QuizResult(
                quizAttemptId: speedAttempt.Id,
                userId: speedAttempt.UserId,
                quizId: speedAttempt.QuizId,
                score: score,
                maxScore: 100,
                correctAnswers: correct,
                totalQuestions: questions,
                timeSpent: fastTime,
                passingThreshold: 60.0
            );

            speedResult.UpdateFeedback("Impressive speed! You scored 92.0% and got 11 out of 12 questions correct in just 8 minutes. Excellent efficiency and knowledge!");

            await context.Set<QuizAttempt>().AddAsync(speedAttempt);
            existingResults.Add(speedResult);
        }

        // Save the additional attempts
        await context.SaveChangesAsync();
    }
}
