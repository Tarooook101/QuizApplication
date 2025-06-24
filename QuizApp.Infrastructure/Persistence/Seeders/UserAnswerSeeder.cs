using Microsoft.EntityFrameworkCore;
using QuizApp.Domain.Entities;
using QuizApp.Domain.Enums;


namespace QuizApp.Infrastructure.Persistence.Seeders;

public static class UserAnswerSeeder
{
    public static async Task SeedAsync(QuizDbContext context)
    {
        if (await context.Set<UserAnswer>().AnyAsync())
        {
            return; // Data already seeded
        }

        // Get existing data needed for seeding
        var quizAttempts = await context.Set<QuizAttempt>()
            .Where(qa => qa.Status == QuizAttemptStatus.Completed)
            .ToListAsync();

        var questions = await context.Set<Question>()
            .Include(q => q.Quiz)
            .ToListAsync();

        var answers = await context.Set<Answer>()
            .Include(a => a.Question)
            .ToListAsync();

        if (!quizAttempts.Any() || !questions.Any())
        {
            return; // No quiz attempts or questions to create user answers for
        }

        var userAnswers = new List<UserAnswer>();
        var random = new Random();

        foreach (var attempt in quizAttempts)
        {
            // Get questions for this specific quiz
            var quizQuestions = questions.Where(q => q.QuizId == attempt.QuizId).OrderBy(q => q.OrderIndex).ToList();

            foreach (var question in quizQuestions)
            {
                UserAnswer userAnswer = null;
                var timeSpent = TimeSpan.FromSeconds(random.Next(30, 300)); // 30 seconds to 5 minutes per question

                switch (question.Type)
                {
                    case QuestionType.MultipleChoice:
                        var mcAnswers = answers.Where(a => a.QuestionId == question.Id).ToList();
                        if (mcAnswers.Any())
                        {
                            // 80% chance to select correct answer, 20% chance for wrong answer
                            var selectedAnswer = random.NextDouble() < 0.8
                                ? mcAnswers.FirstOrDefault(a => a.IsCorrect) ?? mcAnswers.First()
                                : mcAnswers.Where(a => !a.IsCorrect).OrderBy(x => random.Next()).FirstOrDefault() ?? mcAnswers.First();

                            userAnswer = new UserAnswer(
                                quizAttemptId: attempt.Id,
                                questionId: question.Id,
                                selectedAnswerId: selectedAnswer.Id,
                                textAnswer: null,
                                timeSpent: timeSpent
                            );

                            // Set correctness and points using reflection (since properties have private setters)
                            SetUserAnswerProperties(userAnswer, selectedAnswer.IsCorrect, selectedAnswer.IsCorrect ? question.Points : 0);
                        }
                        break;

                    case QuestionType.TrueFalse:
                        var tfAnswers = answers.Where(a => a.QuestionId == question.Id).ToList();
                        if (tfAnswers.Any())
                        {
                            // 85% chance to select correct answer for true/false
                            var selectedAnswer = random.NextDouble() < 0.85
                                ? tfAnswers.FirstOrDefault(a => a.IsCorrect) ?? tfAnswers.First()
                                : tfAnswers.FirstOrDefault(a => !a.IsCorrect) ?? tfAnswers.First();

                            userAnswer = new UserAnswer(
                                quizAttemptId: attempt.Id,
                                questionId: question.Id,
                                selectedAnswerId: selectedAnswer.Id,
                                textAnswer: null,
                                timeSpent: timeSpent
                            );

                            SetUserAnswerProperties(userAnswer, selectedAnswer.IsCorrect, selectedAnswer.IsCorrect ? question.Points : 0);
                        }
                        break;

                    case QuestionType.ShortAnswer:
                        var shortAnswerTexts = GetShortAnswerResponses(question.Text, random);
                        var shortAnswerText = shortAnswerTexts[random.Next(shortAnswerTexts.Length)];
                        var shortAnswerIsCorrect = IsShortAnswerCorrect(question.Text, shortAnswerText);

                        userAnswer = new UserAnswer(
                            quizAttemptId: attempt.Id,
                            questionId: question.Id,
                            selectedAnswerId: null,
                            textAnswer: shortAnswerText,
                            timeSpent: timeSpent
                        );

                        SetUserAnswerProperties(userAnswer, shortAnswerIsCorrect, shortAnswerIsCorrect ? question.Points : (int)(question.Points * 0.5));
                        break;

                    case QuestionType.Essay:
                        var essayResponses = GetEssayResponses(question.Text, random);
                        var essayText = essayResponses[random.Next(essayResponses.Length)];
                        var essayScore = random.Next(1, 4); // 1-3 scale, then convert to points
                        var essayIsCorrect = essayScore >= 2;
                        var essayPoints = (int)((essayScore / 3.0) * question.Points);

                        userAnswer = new UserAnswer(
                            quizAttemptId: attempt.Id,
                            questionId: question.Id,
                            selectedAnswerId: null,
                            textAnswer: essayText,
                            timeSpent: timeSpent
                        );

                        SetUserAnswerProperties(userAnswer, essayIsCorrect, essayPoints);
                        break;

                    case QuestionType.FillInTheBlank:
                        var fillInAnswers = GetFillInTheBlankAnswers(question.Text, random);
                        var fillInAnswer = fillInAnswers[random.Next(fillInAnswers.Length)];
                        var fillInIsCorrect = IsFillInTheBlankCorrect(question.Text, fillInAnswer);

                        userAnswer = new UserAnswer(
                            quizAttemptId: attempt.Id,
                            questionId: question.Id,
                            selectedAnswerId: null,
                            textAnswer: fillInAnswer,
                            timeSpent: timeSpent
                        );

                        SetUserAnswerProperties(userAnswer, fillInIsCorrect, fillInIsCorrect ? question.Points : 0);
                        break;
                }

                if (userAnswer != null)
                {
                    userAnswers.Add(userAnswer);
                }
            }
        }

        // ...

        // Add some partially completed attempts (in-progress)
        var inProgressAttempts = await context.Set<QuizAttempt>()
            .Where(qa => qa.Status == QuizAttemptStatus.InProgress)
            .ToListAsync();

        foreach (var attempt in inProgressAttempts)
        {
            var quizQuestions = questions.Where(q => q.QuizId == attempt.QuizId).OrderBy(q => q.OrderIndex).ToList();

            // Answer only 30-70% of questions for in-progress attempts
            var questionsToAnswer = (int)(quizQuestions.Count * (0.3 + random.NextDouble() * 0.4));
            var selectedQuestions = quizQuestions.Take(questionsToAnswer);

            foreach (var question in selectedQuestions)
            {
                UserAnswer userAnswer = null;
                var timeSpent = TimeSpan.FromSeconds(random.Next(20, 180));

                if (question.Type == QuestionType.MultipleChoice || question.Type == QuestionType.TrueFalse)
                {
                    var mcAnswers = answers.Where(a => a.QuestionId == question.Id).ToList();
                    if (mcAnswers.Any())
                    {
                        var selectedAnswer = mcAnswers[random.Next(mcAnswers.Count)];

                        userAnswer = new UserAnswer(
                            quizAttemptId: attempt.Id,
                            questionId: question.Id,
                            selectedAnswerId: selectedAnswer.Id,
                            textAnswer: null,
                            timeSpent: timeSpent
                        );

                        SetUserAnswerProperties(userAnswer, selectedAnswer.IsCorrect, selectedAnswer.IsCorrect ? question.Points : 0);
                    }
                }
                else
                {
                    // For text-based questions in progress attempts
                    var textAnswer = GetGenericTextAnswer(question.Type, random);
                    userAnswer = new UserAnswer(
                        quizAttemptId: attempt.Id,
                        questionId: question.Id,
                        selectedAnswerId: null,
                        textAnswer: textAnswer,
                        timeSpent: timeSpent
                    );

                    SetUserAnswerProperties(userAnswer, false, 0); // In-progress answers are not graded yet
                }

                if (userAnswer != null)
                {
                    userAnswers.Add(userAnswer);
                }
            }
        }

        if (userAnswers.Any())
        {
            await context.Set<UserAnswer>().AddRangeAsync(userAnswers);
            await context.SaveChangesAsync();
        }
    }

    private static void SetUserAnswerProperties(UserAnswer userAnswer, bool isCorrect, int pointsEarned)
    {
        // Use reflection to set private properties
        var isCorrectProperty = typeof(UserAnswer).GetProperty("IsCorrect");
        var pointsEarnedProperty = typeof(UserAnswer).GetProperty("PointsEarned");

        isCorrectProperty?.SetValue(userAnswer, isCorrect);
        pointsEarnedProperty?.SetValue(userAnswer, pointsEarned);
    }

    private static string[] GetShortAnswerResponses(string questionText, Random random)
    {
        return questionText.ToLower() switch
        {
            var q when q.Contains("function") => new[]
            {
                "To encapsulate reusable code and promote modularity",
                "Functions help organize code into smaller, manageable pieces",
                "A function performs a specific task and can be called multiple times",
                "To avoid code duplication",
                "Functions make code more readable"
            },
            var q when q.Contains("generic") => new[]
            {
                "Type safety and code reusability without sacrificing performance",
                "Generics provide compile-time type checking",
                "They allow writing flexible, reusable code",
                "Better performance than using object type",
                "Eliminates boxing and unboxing"
            },
            var q when q.Contains("acid") => new[]
            {
                "Atomicity, Consistency, Isolation, Durability",
                "ACID properties ensure database transaction reliability",
                "Atomicity Consistency Isolation Durability",
                "Database transaction properties",
                "ACID stands for transaction properties"
            },
            var q when q.Contains("rate limiting") => new[]
            {
                "To prevent abuse and ensure fair usage of API resources",
                "Protect against DoS attacks and server overload",
                "Control the number of requests per time period",
                "Ensure service availability for all users",
                "Prevent API abuse and maintain performance"
            },
            var q when q.Contains("cross-validation") => new[]
            {
                "A technique to assess model performance and prevent overfitting",
                "Cross-validation helps evaluate model generalization",
                "It provides reliable performance estimates",
                "Splits data into training and validation sets multiple times",
                "Helps in model selection and hyperparameter tuning"
            },
            _ => new[]
            {
                "This is a sample answer",
                "Generic response to the question",
                "Standard answer for this type of question",
                "Typical response",
                "Common answer"
            }
        };
    }

    private static bool IsShortAnswerCorrect(string questionText, string answer)
    {
        return questionText.ToLower() switch
        {
            var q when q.Contains("function") => answer.Contains("reusable") || answer.Contains("modular") || answer.Contains("encapsulate"),
            var q when q.Contains("generic") => answer.Contains("type safety") || answer.Contains("reusability") || answer.Contains("performance"),
            var q when q.Contains("acid") => answer.Contains("Atomicity") && answer.Contains("Consistency"),
            var q when q.Contains("rate limiting") => answer.Contains("prevent") || answer.Contains("abuse") || answer.Contains("DoS"),
            var q when q.Contains("cross-validation") => answer.Contains("performance") || answer.Contains("overfitting") || answer.Contains("model"),
            _ => new Random().NextDouble() < 0.7 // 70% chance of being correct for unknown questions
        };
    }

    private static string[] GetEssayResponses(string questionText, Random random)
    {
        return questionText.ToLower() switch
        {
            var q when q.Contains("ienumerable") && q.Contains("iqueryable") => new[]
            {
                "IEnumerable<T> executes queries in memory using LINQ to Objects, while IQueryable<T> can translate expressions to other query languages like SQL. IEnumerable is best for in-memory collections, while IQueryable is ideal for database queries as it supports deferred execution and can optimize queries at the data source level.",
                "The main difference is that IEnumerable processes data in memory, while IQueryable can work with external data sources. IQueryable builds expression trees that can be translated to SQL, making it more efficient for database operations.",
                "IEnumerable is for in-memory operations, IQueryable is for database queries. IQueryable supports expression trees and can be translated to SQL."
            },
            var q when q.Contains("inner join") && q.Contains("left join") => new[]
            {
                "INNER JOIN returns only rows that have matching values in both tables, while LEFT JOIN returns all rows from the left table and matching rows from the right table. When there's no match in the right table, LEFT JOIN returns NULL values for the right table columns.",
                "INNER JOIN shows only matching records from both tables. LEFT JOIN shows all records from the left table plus matches from the right table, with NULLs where no match exists.",
                "The key difference is that INNER JOIN filters out non-matching rows, while LEFT JOIN preserves all rows from the left table regardless of matches."
            },
            var q when q.Contains("cors") => new[]
            {
                "CORS (Cross-Origin Resource Sharing) is a security mechanism that allows web applications running on one domain to access resources from another domain. It overcomes the same-origin policy restrictions by using HTTP headers to tell browsers which origins are allowed to access the API.",
                "CORS enables controlled access to resources across different domains. It's essential for web APIs that need to be accessed by frontend applications running on different domains.",
                "CORS allows secure cross-domain requests by using specific HTTP headers to indicate which origins, methods, and headers are permitted."
            },
            var q when q.Contains("supervised") && q.Contains("unsupervised") => new[]
            {
                "Supervised learning uses labeled training data to learn the mapping between inputs and outputs. Examples include classification (spam detection) and regression (price prediction). Unsupervised learning finds patterns in unlabeled data without target variables. Examples include clustering (customer segmentation) and dimensionality reduction (PCA).",
                "Supervised learning requires labeled data and aims to predict outcomes. Unsupervised learning discovers hidden patterns in unlabeled data. Supervised examples: classification, regression. Unsupervised examples: clustering, association rules.",
                "The main difference is the presence of labels. Supervised learning learns from examples with known outcomes, while unsupervised learning finds structure in data without predefined answers."
            },
            _ => new[]
            {
                "This is a comprehensive essay response that covers the key points of the question. It demonstrates understanding of the topic and provides relevant examples and explanations.",
                "A detailed answer that addresses the main concepts and provides practical insights into the subject matter.",
                "An essay response that shows good knowledge of the topic with appropriate examples and clear explanations."
            }
        };
    }

    private static string[] GetFillInTheBlankAnswers(string questionText, Random random)
    {
        return questionText.ToLower() switch
        {
            var q when q.Contains("algorithm") => new[] { "algorithm", "Algorithm", "procedure", "process" },
            var q when q.Contains("delegate") && q.Contains("methods") => new[] { "multiple", "Multiple", "many", "several" },
            var q when q.Contains("first normal form") => new[] { "atomic", "Atomic", "single", "indivisible" },
            var q when q.Contains("https") && q.Contains("encryption") => new[] { "SSL/TLS", "TLS", "SSL", "transport layer security" },
            var q when q.Contains("feature") => new[] { "selection", "Selection", "engineering", "extraction" },
            _ => new[] { "answer", "Answer", "response", "value" }
        };
    }

    private static bool IsFillInTheBlankCorrect(string questionText, string answer)
    {
        return questionText.ToLower() switch
        {
            var q when q.Contains("algorithm") => answer.ToLower().Contains("algorithm") || answer.ToLower().Contains("procedure"),
            var q when q.Contains("delegate") => answer.ToLower().Contains("multiple") || answer.ToLower().Contains("many"),
            var q when q.Contains("first normal form") => answer.ToLower().Contains("atomic") || answer.ToLower().Contains("single"),
            var q when q.Contains("https") => answer.ToLower().Contains("ssl") || answer.ToLower().Contains("tls"),
            var q when q.Contains("feature") => answer.ToLower().Contains("selection") || answer.ToLower().Contains("engineering"),
            _ => new Random().NextDouble() < 0.6 // 60% chance for unknown questions
        };
    }

    private static string GetGenericTextAnswer(QuestionType questionType, Random random)
    {
        return questionType switch
        {
            QuestionType.ShortAnswer => "This is a work in progress answer...",
            QuestionType.Essay => "I'm still working on this essay response. Need more time to provide a comprehensive answer.",
            QuestionType.FillInTheBlank => "incomplete",
            _ => "In progress..."
        };
    }
}