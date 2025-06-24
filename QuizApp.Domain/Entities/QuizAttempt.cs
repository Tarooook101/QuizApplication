using QuizApp.Domain.Common;
using QuizApp.Domain.Enums;
using QuizApp.Domain.Events.QuizAttemptEvents;


namespace QuizApp.Domain.Entities;

public class QuizAttempt : BaseEntity<Guid>
{
    public Guid QuizId { get; private set; }
    public Guid UserId { get; private set; }
    public DateTime StartedAt { get; private set; }
    public DateTime? CompletedAt { get; private set; }
    public QuizAttemptStatus Status { get; private set; }
    public int? Score { get; private set; }
    public int? MaxScore { get; private set; }
    public double? Percentage { get; private set; }
    public int TimeSpentMinutes { get; private set; }
    public string? Notes { get; private set; }

    private QuizAttempt() : base() { }

    public QuizAttempt(Guid quizId, Guid userId) : base(Guid.NewGuid())
    {
        SetQuizId(quizId);
        SetUserId(userId);
        StartedAt = DateTime.UtcNow;
        Status = QuizAttemptStatus.InProgress;
        TimeSpentMinutes = 0;

        AddDomainEvent(new QuizAttemptStartedEvent(this));
    }

    public void Complete(int score, int maxScore, string? notes = null)
    {
        if (Status != QuizAttemptStatus.InProgress)
            throw new InvalidOperationException("Only in-progress quiz attempts can be completed");

        CompletedAt = DateTime.UtcNow;
        Status = QuizAttemptStatus.Completed;
        SetScore(score);
        SetMaxScore(maxScore);
        CalculatePercentage();
        CalculateTimeSpent();
        SetNotes(notes);
        MarkAsUpdated();

        AddDomainEvent(new QuizAttemptCompletedEvent(this));
    }

    public void Abandon()
    {
        if (Status != QuizAttemptStatus.InProgress)
            throw new InvalidOperationException("Only in-progress quiz attempts can be abandoned");

        Status = QuizAttemptStatus.Abandoned;
        CalculateTimeSpent();
        MarkAsUpdated();

        AddDomainEvent(new QuizAttemptAbandonedEvent(this));
    }

    public void UpdateProgress(string? notes = null)
    {
        if (Status != QuizAttemptStatus.InProgress)
            throw new InvalidOperationException("Only in-progress quiz attempts can be updated");

        SetNotes(notes);
        MarkAsUpdated();
    }

    private void SetQuizId(Guid quizId)
    {
        if (quizId == Guid.Empty)
            throw new ArgumentException("Quiz ID cannot be empty", nameof(quizId));

        QuizId = quizId;
    }

    private void SetUserId(Guid userId)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException("User ID cannot be empty", nameof(userId));

        UserId = userId;
    }

    private void SetScore(int score)
    {
        if (score < 0)
            throw new ArgumentException("Score cannot be negative", nameof(score));

        Score = score;
    }

    private void SetMaxScore(int maxScore)
    {
        if (maxScore <= 0)
            throw new ArgumentException("Max score must be positive", nameof(maxScore));

        MaxScore = maxScore;
    }

    private void CalculatePercentage()
    {
        if (Score.HasValue && MaxScore.HasValue && MaxScore.Value > 0)
        {
            Percentage = Math.Round((double)Score.Value / MaxScore.Value * 100, 2);
        }
    }

    private void CalculateTimeSpent()
    {
        var endTime = CompletedAt ?? DateTime.UtcNow;
        TimeSpentMinutes = (int)Math.Ceiling((endTime - StartedAt).TotalMinutes);
    }

    private void SetNotes(string? notes)
    {
        if (!string.IsNullOrEmpty(notes) && notes.Length > 1000)
            throw new ArgumentException("Notes cannot exceed 1000 characters", nameof(notes));

        Notes = notes?.Trim();
    }
}
