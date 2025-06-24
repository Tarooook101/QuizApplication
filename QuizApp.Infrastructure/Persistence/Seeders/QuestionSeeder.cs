using Microsoft.EntityFrameworkCore;
using QuizApp.Domain.Entities;
using QuizApp.Domain.Enums;


namespace QuizApp.Infrastructure.Persistence.Seeders;

public static class QuestionSeeder
{
    public static async Task SeedAsync(QuizDbContext context)
    {
        if (await context.Set<Question>().AnyAsync())
        {
            return; // Data already seeded
        }

        // Get all existing quizzes
        var quizzes = await context.Set<Quiz>().ToListAsync();
        if (!quizzes.Any())
        {
            return; // No quizzes to add questions to
        }

        var questions = new List<Question>();

        // Questions for "Basic Programming Concepts" quiz
        var basicProgrammingQuiz = quizzes.FirstOrDefault(q => q.Title == "Basic Programming Concepts");
        if (basicProgrammingQuiz != null)
        {
            questions.AddRange(new[]
            {
                new Question(
                    text: "What is a variable in programming?",
                    type: QuestionType.MultipleChoice,
                    points: 5,
                    orderIndex: 1,
                    quizId: basicProgrammingQuiz.Id,
                    isRequired: true,
                    explanation: "A variable is a named storage location that can hold data values during program execution.",
                    imageUrl: null
                ),

                new Question(
                    text: "Which of the following is NOT a primitive data type in most programming languages?",
                    type: QuestionType.MultipleChoice,
                    points: 5,
                    orderIndex: 2,
                    quizId: basicProgrammingQuiz.Id,
                    isRequired: true,
                    explanation: "Arrays are composite data types, not primitive. Primitive types include int, char, boolean, etc.",
                    imageUrl: null
                ),

                new Question(
                    text: "A for loop can be used to repeat a block of code a specific number of times.",
                    type: QuestionType.TrueFalse,
                    points: 3,
                    orderIndex: 3,
                    quizId: basicProgrammingQuiz.Id,
                    isRequired: true,
                    explanation: "True. For loops are specifically designed for controlled repetition with a known number of iterations.",
                    imageUrl: null
                ),

                new Question(
                    text: "What is the purpose of a function in programming?",
                    type: QuestionType.ShortAnswer,
                    points: 8,
                    orderIndex: 4,
                    quizId: basicProgrammingQuiz.Id,
                    isRequired: true,
                    explanation: "Functions encapsulate reusable code blocks, promote modularity, and help organize program logic.",
                    imageUrl: null
                ),

                new Question(
                    text: "Complete the following: 'An _____ is a step-by-step procedure for solving a problem.'",
                    type: QuestionType.FillInTheBlank,
                    points: 4,
                    orderIndex: 5,
                    quizId: basicProgrammingQuiz.Id,
                    isRequired: true,
                    explanation: "Algorithm - a finite set of instructions for solving a computational problem.",
                    imageUrl: null
                )
            });
        }

        // Questions for "Advanced C# Features" quiz
        var advancedCSharpQuiz = quizzes.FirstOrDefault(q => q.Title == "Advanced C# Features");
        if (advancedCSharpQuiz != null)
        {
            questions.AddRange(new[]
            {
                new Question(
                    text: "Which LINQ method would you use to filter a collection based on a condition?",
                    type: QuestionType.MultipleChoice,
                    points: 6,
                    orderIndex: 1,
                    quizId: advancedCSharpQuiz.Id,
                    isRequired: true,
                    explanation: "Where() method is used to filter collections based on a predicate condition.",
                    imageUrl: null
                ),

                new Question(
                    text: "The 'async' and 'await' keywords in C# are used for asynchronous programming.",
                    type: QuestionType.TrueFalse,
                    points: 4,
                    orderIndex: 2,
                    quizId: advancedCSharpQuiz.Id,
                    isRequired: true,
                    explanation: "True. These keywords enable writing asynchronous code that doesn't block the calling thread.",
                    imageUrl: null
                ),

                new Question(
                    text: "Explain the difference between IEnumerable<T> and IQueryable<T> in C#.",
                    type: QuestionType.Essay,
                    points: 15,
                    orderIndex: 3,
                    quizId: advancedCSharpQuiz.Id,
                    isRequired: true,
                    explanation: "IEnumerable executes queries in memory, while IQueryable can translate queries to other languages like SQL.",
                    imageUrl: null
                ),

                new Question(
                    text: "What is the primary benefit of using generics in C#?",
                    type: QuestionType.ShortAnswer,
                    points: 8,
                    orderIndex: 4,
                    quizId: advancedCSharpQuiz.Id,
                    isRequired: true,
                    explanation: "Type safety, performance improvement, and code reusability without sacrificing type checking.",
                    imageUrl: null
                ),

                new Question(
                    text: "A delegate in C# can hold references to _____ methods.",
                    type: QuestionType.FillInTheBlank,
                    points: 5,
                    orderIndex: 5,
                    quizId: advancedCSharpQuiz.Id,
                    isRequired: true,
                    explanation: "Multiple - delegates can hold references to multiple methods (multicast delegates).",
                    imageUrl: null
                )
            });
        }

        // Questions for "Database Design Principles" quiz
        var databaseDesignQuiz = quizzes.FirstOrDefault(q => q.Title == "Database Design Principles");
        if (databaseDesignQuiz != null)
        {
            questions.AddRange(new[]
            {
                new Question(
                    text: "What is the main purpose of database normalization?",
                    type: QuestionType.MultipleChoice,
                    points: 7,
                    orderIndex: 1,
                    quizId: databaseDesignQuiz.Id,
                    isRequired: true,
                    explanation: "Normalization reduces data redundancy and eliminates insertion, update, and deletion anomalies.",
                    imageUrl: null
                ),

                new Question(
                    text: "A foreign key can reference a primary key in the same table.",
                    type: QuestionType.TrueFalse,
                    points: 4,
                    orderIndex: 2,
                    quizId: databaseDesignQuiz.Id,
                    isRequired: true,
                    explanation: "True. This creates a self-referencing relationship, commonly used for hierarchical data.",
                    imageUrl: null
                ),

                new Question(
                    text: "Describe the difference between INNER JOIN and LEFT JOIN in SQL.",
                    type: QuestionType.Essay,
                    points: 12,
                    orderIndex: 3,
                    quizId: databaseDesignQuiz.Id,
                    isRequired: true,
                    explanation: "INNER JOIN returns only matching records, LEFT JOIN returns all records from left table plus matches from right.",
                    imageUrl: null
                ),

                new Question(
                    text: "What does ACID stand for in database transactions?",
                    type: QuestionType.ShortAnswer,
                    points: 8,
                    orderIndex: 4,
                    quizId: databaseDesignQuiz.Id,
                    isRequired: true,
                    explanation: "Atomicity, Consistency, Isolation, Durability - the four properties of database transactions.",
                    imageUrl: null
                ),

                new Question(
                    text: "First Normal Form (1NF) requires that each column contains _____ values.",
                    type: QuestionType.FillInTheBlank,
                    points: 5,
                    orderIndex: 5,
                    quizId: databaseDesignQuiz.Id,
                    isRequired: true,
                    explanation: "Atomic - each column should contain indivisible, single values.",
                    imageUrl: null
                )
            });
        }

        // Questions for "Web API Security" quiz
        var webApiSecurityQuiz = quizzes.FirstOrDefault(q => q.Title == "Web API Security");
        if (webApiSecurityQuiz != null)
        {
            questions.AddRange(new[]
            {
                new Question(
                    text: "Which HTTP status code should be returned when authentication fails?",
                    type: QuestionType.MultipleChoice,
                    points: 5,
                    orderIndex: 1,
                    quizId: webApiSecurityQuiz.Id,
                    isRequired: true,
                    explanation: "401 Unauthorized indicates that authentication is required and has failed or not been provided.",
                    imageUrl: null
                ),

                new Question(
                    text: "JWT tokens are stateless and contain encoded user information.",
                    type: QuestionType.TrueFalse,
                    points: 4,
                    orderIndex: 2,
                    quizId: webApiSecurityQuiz.Id,
                    isRequired: true,
                    explanation: "True. JWT tokens are self-contained and include claims about the user without server-side session storage.",
                    imageUrl: null
                ),

                new Question(
                    text: "Explain the purpose of CORS (Cross-Origin Resource Sharing) in web APIs.",
                    type: QuestionType.Essay,
                    points: 10,
                    orderIndex: 3,
                    quizId: webApiSecurityQuiz.Id,
                    isRequired: true,
                    explanation: "CORS allows web applications running on one domain to access resources from another domain, overcoming same-origin policy restrictions.",
                    imageUrl: null
                ),

                new Question(
                    text: "What is the primary purpose of API rate limiting?",
                    type: QuestionType.ShortAnswer,
                    points: 6,
                    orderIndex: 4,
                    quizId: webApiSecurityQuiz.Id,
                    isRequired: true,
                    explanation: "To prevent abuse, ensure fair usage, and protect against DoS attacks by limiting request frequency.",
                    imageUrl: null
                ),

                new Question(
                    text: "HTTPS encrypts data using _____ encryption during transmission.",
                    type: QuestionType.FillInTheBlank,
                    points: 4,
                    orderIndex: 5,
                    quizId: webApiSecurityQuiz.Id,
                    isRequired: true,
                    explanation: "SSL/TLS - Secure Sockets Layer/Transport Layer Security protocols.",
                    imageUrl: null
                )
            });
        }

        // Questions for "Machine Learning Fundamentals" quiz
        var mlFundamentalsQuiz = quizzes.FirstOrDefault(q => q.Title == "Machine Learning Fundamentals");
        if (mlFundamentalsQuiz != null)
        {
            questions.AddRange(new[]
            {
                new Question(
                    text: "Which type of machine learning algorithm learns from labeled training data?",
                    type: QuestionType.MultipleChoice,
                    points: 6,
                    orderIndex: 1,
                    quizId: mlFundamentalsQuiz.Id,
                    isRequired: true,
                    explanation: "Supervised learning algorithms use labeled datasets to learn the mapping between inputs and outputs.",
                    imageUrl: null
                ),

                new Question(
                    text: "Overfitting occurs when a model performs well on training data but poorly on new data.",
                    type: QuestionType.TrueFalse,
                    points: 5,
                    orderIndex: 2,
                    quizId: mlFundamentalsQuiz.Id,
                    isRequired: true,
                    explanation: "True. Overfitting happens when a model learns training data too specifically and fails to generalize.",
                    imageUrl: null
                ),

                new Question(
                    text: "Compare and contrast supervised learning with unsupervised learning, providing examples of each.",
                    type: QuestionType.Essay,
                    points: 20,
                    orderIndex: 3,
                    quizId: mlFundamentalsQuiz.Id,
                    isRequired: true,
                    explanation: "Supervised uses labeled data (classification, regression), unsupervised finds patterns in unlabeled data (clustering, dimensionality reduction).",
                    imageUrl: null
                ),

                new Question(
                    text: "What is cross-validation and why is it important in machine learning?",
                    type: QuestionType.ShortAnswer,
                    points: 10,
                    orderIndex: 4,
                    quizId: mlFundamentalsQuiz.Id,
                    isRequired: true,
                    explanation: "Cross-validation assesses model performance by splitting data into folds, ensuring reliable evaluation and preventing overfitting.",
                    imageUrl: null
                ),

                new Question(
                    text: "The process of selecting relevant features from a dataset is called feature _____.",
                    type: QuestionType.FillInTheBlank,
                    points: 5,
                    orderIndex: 5,
                    quizId: mlFundamentalsQuiz.Id,
                    isRequired: true,
                    explanation: "Selection - the process of identifying and selecting the most relevant features for model training.",
                    imageUrl: null
                )
            });
        }

        // Add all questions to the context
        if (questions.Any())
        {
            await context.Set<Question>().AddRangeAsync(questions);
            await context.SaveChangesAsync();
        }
    }
}