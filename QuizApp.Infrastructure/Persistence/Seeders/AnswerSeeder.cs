using Microsoft.EntityFrameworkCore;
using QuizApp.Domain.Entities;
using QuizApp.Domain.Enums;

namespace QuizApp.Infrastructure.Persistence.Seeders;

public static class AnswerSeeder
{
    public static async Task SeedAsync(QuizDbContext context)
    {
        if (await context.Set<Answer>().AnyAsync())
        {
            return; // Data already seeded
        }

        // Get all existing questions
        var questions = await context.Set<Question>()
            .Include(q => q.Quiz)
            .ToListAsync();

        if (!questions.Any())
        {
            return; // No questions to add answers to
        }

        var answers = new List<Answer>();

        // Seed answers for Multiple Choice and True/False questions only
        foreach (var question in questions.Where(q => q.Type == QuestionType.MultipleChoice || q.Type == QuestionType.TrueFalse))
        {
            switch (question.Quiz.Title)
            {
                case "Basic Programming Concepts":
                    answers.AddRange(GetBasicProgrammingAnswers(question));
                    break;

                case "Advanced C# Features":
                    answers.AddRange(GetAdvancedCSharpAnswers(question));
                    break;

                case "Database Design Principles":
                    answers.AddRange(GetDatabaseDesignAnswers(question));
                    break;

                case "Web API Security":
                    answers.AddRange(GetWebApiSecurityAnswers(question));
                    break;

                case "Machine Learning Fundamentals":
                    answers.AddRange(GetMachineLearningAnswers(question));
                    break;
            }
        }

        // Add all answers to the context
        if (answers.Any())
        {
            await context.Set<Answer>().AddRangeAsync(answers);
            await context.SaveChangesAsync();
        }
    }

    private static List<Answer> GetBasicProgrammingAnswers(Question question)
    {
        var answers = new List<Answer>();

        switch (question.Text)
        {
            case "What is a variable in programming?":
                answers.AddRange(new[]
                {
                    new Answer("A named storage location that can hold data values", true, 1, question.Id, "Correct! Variables are named containers for storing data."),
                    new Answer("A mathematical operation", false, 2, question.Id, "Incorrect. This describes an operator or function."),
                    new Answer("A type of loop structure", false, 3, question.Id, "Incorrect. This describes a control flow statement."),
                    new Answer("A programming language", false, 4, question.Id, "Incorrect. This describes a tool for writing programs.")
                });
                break;

            case "Which of the following is NOT a primitive data type in most programming languages?":
                answers.AddRange(new[]
                {
                    new Answer("Integer", false, 1, question.Id, "Incorrect. Integer is a primitive data type."),
                    new Answer("Boolean", false, 2, question.Id, "Incorrect. Boolean is a primitive data type."),
                    new Answer("Character", false, 3, question.Id, "Incorrect. Character is a primitive data type."),
                    new Answer("Array", true, 4, question.Id, "Correct! Arrays are composite/reference types, not primitive types.")
                });
                break;

            case "A for loop can be used to repeat a block of code a specific number of times.":
                answers.AddRange(new[]
                {
                    new Answer("True", true, 1, question.Id, "Correct! For loops are designed for controlled repetition."),
                    new Answer("False", false, 2, question.Id, "Incorrect. For loops are specifically used for this purpose.")
                });
                break;
        }

        return answers;
    }

    private static List<Answer> GetAdvancedCSharpAnswers(Question question)
    {
        var answers = new List<Answer>();

        switch (question.Text)
        {
            case "Which LINQ method would you use to filter a collection based on a condition?":
                answers.AddRange(new[]
                {
                    new Answer("Select()", false, 1, question.Id, "Incorrect. Select is used for projection/transformation."),
                    new Answer("Where()", true, 2, question.Id, "Correct! Where() method filters collections based on a predicate."),
                    new Answer("OrderBy()", false, 3, question.Id, "Incorrect. OrderBy is used for sorting."),
                    new Answer("GroupBy()", false, 4, question.Id, "Incorrect. GroupBy is used for grouping elements.")
                });
                break;

            case "The 'async' and 'await' keywords in C# are used for asynchronous programming.":
                answers.AddRange(new[]
                {
                    new Answer("True", true, 1, question.Id, "Correct! These keywords enable non-blocking asynchronous operations."),
                    new Answer("False", false, 2, question.Id, "Incorrect. This is exactly what async/await are designed for.")
                });
                break;
        }

        return answers;
    }

    private static List<Answer> GetDatabaseDesignAnswers(Question question)
    {
        var answers = new List<Answer>();

        switch (question.Text)
        {
            case "What is the main purpose of database normalization?":
                answers.AddRange(new[]
                {
                    new Answer("To increase data redundancy", false, 1, question.Id, "Incorrect. Normalization reduces redundancy."),
                    new Answer("To reduce data redundancy and eliminate anomalies", true, 2, question.Id, "Correct! Normalization eliminates insertion, update, and deletion anomalies."),
                    new Answer("To make queries run slower", false, 3, question.Id, "Incorrect. This is not the purpose of normalization."),
                    new Answer("To increase storage space usage", false, 4, question.Id, "Incorrect. Normalization typically reduces storage requirements.")
                });
                break;

            case "A foreign key can reference a primary key in the same table.":
                answers.AddRange(new[]
                {
                    new Answer("True", true, 1, question.Id, "Correct! This creates self-referencing relationships for hierarchical data."),
                    new Answer("False", false, 2, question.Id, "Incorrect. Self-referencing foreign keys are allowed and useful.")
                });
                break;
        }

        return answers;
    }

    private static List<Answer> GetWebApiSecurityAnswers(Question question)
    {
        var answers = new List<Answer>();

        switch (question.Text)
        {
            case "Which HTTP status code should be returned when authentication fails?":
                answers.AddRange(new[]
                {
                    new Answer("400 Bad Request", false, 1, question.Id, "Incorrect. 400 indicates a client error with the request format."),
                    new Answer("401 Unauthorized", true, 2, question.Id, "Correct! 401 indicates authentication failure or missing credentials."),
                    new Answer("403 Forbidden", false, 3, question.Id, "Incorrect. 403 indicates authorization failure, not authentication."),
                    new Answer("404 Not Found", false, 4, question.Id, "Incorrect. 404 indicates the resource was not found.")
                });
                break;

            case "JWT tokens are stateless and contain encoded user information.":
                answers.AddRange(new[]
                {
                    new Answer("True", true, 1, question.Id, "Correct! JWT tokens are self-contained and don't require server-side session storage."),
                    new Answer("False", false, 2, question.Id, "Incorrect. This is a key characteristic of JWT tokens.")
                });
                break;
        }

        return answers;
    }

    private static List<Answer> GetMachineLearningAnswers(Question question)
    {
        var answers = new List<Answer>();

        switch (question.Text)
        {
            case "Which type of machine learning algorithm learns from labeled training data?":
                answers.AddRange(new[]
                {
                    new Answer("Unsupervised Learning", false, 1, question.Id, "Incorrect. Unsupervised learning works with unlabeled data."),
                    new Answer("Supervised Learning", true, 2, question.Id, "Correct! Supervised learning uses labeled datasets to learn input-output mappings."),
                    new Answer("Reinforcement Learning", false, 3, question.Id, "Incorrect. Reinforcement learning learns through rewards and penalties."),
                    new Answer("Semi-supervised Learning", false, 4, question.Id, "Partially correct, but supervised learning is the primary answer.")
                });
                break;

            case "Overfitting occurs when a model performs well on training data but poorly on new data.":
                answers.AddRange(new[]
                {
                    new Answer("True", true, 1, question.Id, "Correct! Overfitting means the model has memorized training data but can't generalize."),
                    new Answer("False", false, 2, question.Id, "Incorrect. This is the exact definition of overfitting.")
                });
                break;
        }

        return answers;
    }
}