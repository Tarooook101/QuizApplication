using Microsoft.EntityFrameworkCore;
using QuizApp.API.Middleware;
using QuizApp.Infrastructure.Persistence;
using QuizApp.Infrastructure.Persistence.Seeders;
using Serilog;

namespace QuizApp.API.Extensions;

public static class WebApplicationExtensions
{
    public static WebApplication ConfigurePipeline(this WebApplication app)
    {
        // Enable Swagger in all environments (you can restrict to Development if needed)
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "QuizApp API V1");
            c.RoutePrefix = string.Empty; // Serve Swagger at root URL
            c.DisplayRequestDuration();
            c.EnableDeepLinking();
            c.EnableFilter();
            c.ShowExtensions();
            c.EnableValidator();
        });

        // Exception handling middleware
        app.UseMiddleware<ExceptionHandlingMiddleware>();

        // Security headers
        app.UseHttpsRedirection();

        // CORS
        app.UseCors("AllowAll");

        // Authentication & Authorization
        app.UseAuthentication();
        app.UseAuthorization();

        // Map controllers
        app.MapControllers();

        return app;
    }

    public static async Task<WebApplication> MigrateDatabaseAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        try
        {
            var context = scope.ServiceProvider.GetRequiredService<QuizDbContext>();
            await context.Database.MigrateAsync();

            await CategorySeeder.SeedAsync(context);

            await QuizSeeder.SeedAsync(context);

            await QuestionSeeder.SeedAsync(context);

            await AnswerSeeder.SeedAsync(context);

            await QuizAttemptSeeder.SeedAsync(context);

            await UserAnswerSeeder.SeedAsync(context);
            await QuizResultSeeder.SeedAsync(context);
            await QuizReviewSeeder.SeedAsync(context);

            Log.Information("Database migration and seeding completed successfully");
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "An error occurred while migrating the database");
            throw;
        }

        return app;
    }
}