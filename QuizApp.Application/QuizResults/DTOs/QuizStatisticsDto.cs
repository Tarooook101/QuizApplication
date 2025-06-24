namespace QuizApp.Application.QuizResults.DTOs;

public class QuizStatisticsDto
{
    public Guid QuizId { get; set; }
    public int TotalAttempts { get; set; }
    public double AverageScore { get; set; }
    public double AveragePercentage { get; set; }
    public int PassedCount { get; set; }
    public int FailedCount { get; set; }
    public double PassRate { get; set; }
    public int HighestScore { get; set; }
    public int LowestScore { get; set; }
    public TimeSpan AverageTimeSpent { get; set; }
}