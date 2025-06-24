using Microsoft.EntityFrameworkCore;
using QuizApp.Domain.Entities;


namespace QuizApp.Infrastructure.Persistence.Seeders;

public static class CategorySeeder
{
    public static async Task SeedAsync(QuizDbContext context)
    {
        if (await context.Set<Category>().AnyAsync())
            return;

        var categories = new List<Category>
        {
            new("Science", "Questions related to scientific concepts, discoveries, and natural phenomena", "/icons/science.svg", 1, "#10b981"),
            new("History", "Questions about historical events, figures, and civilizations", "/icons/history.svg", 2, "#f59e0b"),
            new("Technology", "Questions about computers, programming, and technological innovations", "/icons/technology.svg", 3, "#3b82f6"),
            new("Sports", "Questions about various sports, athletes, and competitions", "/icons/sports.svg", 4, "#ef4444"),
            new("Geography", "Questions about countries, capitals, landmarks, and physical features", "/icons/geography.svg", 5, "#8b5cf6"),
            new("Literature", "Questions about books, authors, and literary works", "/icons/literature.svg", 6, "#ec4899"),
            new("Movies & TV", "Questions about films, television shows, and entertainment", "/icons/entertainment.svg", 7, "#f97316"),
            new("Music", "Questions about musicians, songs, and musical instruments", "/icons/music.svg", 8, "#06b6d4"),
            new("Art", "Questions about paintings, sculptures, and famous artists", "/icons/art.svg", 9, "#84cc16"),
            new("General Knowledge", "Mixed questions from various topics and fields", "/icons/general.svg", 10, "#6366f1")
        };

        await context.Set<Category>().AddRangeAsync(categories);
        await context.SaveChangesAsync();
    }
}

