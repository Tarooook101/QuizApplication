using QuizApp.Domain.Common;
using QuizApp.Domain.Enums;
using QuizApp.Domain.Events.QuizResultEvents;


namespace QuizApp.Domain.Entities;

public class QuizResult : BaseEntity<Guid>
{
    public Guid QuizAttemptId { get; private set; }
    public Guid UserId { get; private set; }
    public Guid QuizId { get; private set; }
    public int Score { get; private set; }
    public int MaxScore { get; private set; }
    public double Percentage { get; private set; }
    public int CorrectAnswers { get; private set; }
    public int TotalQuestions { get; private set; }
    public TimeSpan TimeSpent { get; private set; }
    public QuizResultStatus Status { get; private set; }
    public string? Feedback { get; private set; }
    public DateTime CompletedAt { get; private set; }
    public bool IsPassed { get; private set; }
    public double? PassingThreshold { get; private set; }

    // Navigation properties
    public QuizAttempt QuizAttempt { get; private set; } = null!;
    public ApplicationUser User { get; private set; } = null!;

    private QuizResult() : base() { }

    public QuizResult(
        Guid quizAttemptId,
        Guid userId,
        Guid quizId,
        int score,
        int maxScore,
        int correctAnswers,
        int totalQuestions,
        TimeSpan timeSpent,
        double? passingThreshold = null) : base(Guid.NewGuid())
    {
        SetQuizAttemptId(quizAttemptId);
        SetUserId(userId);
        SetQuizId(quizId);
        SetScore(score, maxScore);
        SetAnswerCounts(correctAnswers, totalQuestions);
        SetTimeSpent(timeSpent);
        SetPassingThreshold(passingThreshold);
        CalculatePercentage();
        DetermineStatus();
        CompletedAt = DateTime.UtcNow;

        AddDomainEvent(new QuizResultCreatedEvent(this));
    }

    public void UpdateFeedback(string feedback)
    {
        SetFeedback(feedback);
        MarkAsUpdated();
        AddDomainEvent(new QuizResultUpdatedEvent(this));
    }

    public void Recalculate(int score, int maxScore, int correctAnswers, int totalQuestions)
    {
        SetScore(score, maxScore);
        SetAnswerCounts(correctAnswers, totalQuestions);
        CalculatePercentage();
        DetermineStatus();
        MarkAsUpdated();
        AddDomainEvent(new QuizResultUpdatedEvent(this));
    }

    private void SetQuizAttemptId(Guid quizAttemptId)
    {
        if (quizAttemptId == Guid.Empty)
            throw new ArgumentException("Quiz attempt ID cannot be empty", nameof(quizAttemptId));
        QuizAttemptId = quizAttemptId;
    }

    private void SetUserId(Guid userId)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException("User ID cannot be empty", nameof(userId));
        UserId = userId;
    }

    private void SetQuizId(Guid quizId)
    {
        if (quizId == Guid.Empty)
            throw new ArgumentException("Quiz ID cannot be empty", nameof(quizId));
        QuizId = quizId;
    }

    private void SetScore(int score, int maxScore)
    {
        if (score < 0)
            throw new ArgumentException("Score cannot be negative", nameof(score));
        if (maxScore <= 0)
            throw new ArgumentException("Max score must be positive", nameof(maxScore));
        if (score > maxScore)
            throw new ArgumentException("Score cannot exceed max score", nameof(score));

        Score = score;
        MaxScore = maxScore;
    }

    private void SetAnswerCounts(int correctAnswers, int totalQuestions)
    {
        if (correctAnswers < 0)
            throw new ArgumentException("Correct answers cannot be negative", nameof(correctAnswers));
        if (totalQuestions <= 0)
            throw new ArgumentException("Total questions must be positive", nameof(totalQuestions));
        if (correctAnswers > totalQuestions)
            throw new ArgumentException("Correct answers cannot exceed total questions", nameof(correctAnswers));

        CorrectAnswers = correctAnswers;
        TotalQuestions = totalQuestions;
    }

    private void SetTimeSpent(TimeSpan timeSpent)
    {
        if (timeSpent < TimeSpan.Zero)
            throw new ArgumentException("Time spent cannot be negative", nameof(timeSpent));
        TimeSpent = timeSpent;
    }

    private void SetPassingThreshold(double? passingThreshold)
    {
        if (passingThreshold.HasValue && (passingThreshold < 0 || passingThreshold > 100))
            throw new ArgumentException("Passing threshold must be between 0 and 100", nameof(passingThreshold));
        PassingThreshold = passingThreshold;
    }

    private void SetFeedback(string feedback)
    {
        if (!string.IsNullOrEmpty(feedback) && feedback.Length > 2000)
            throw new ArgumentException("Feedback cannot exceed 2000 characters", nameof(feedback));
        Feedback = feedback?.Trim();
    }

    private void CalculatePercentage()
    {
        Percentage = Math.Round((double)Score / MaxScore * 100, 2);
    }

    private void DetermineStatus()
    {
        if (PassingThreshold.HasValue)
        {
            IsPassed = Percentage >= PassingThreshold.Value;
            Status = IsPassed ? QuizResultStatus.Passed : QuizResultStatus.Failed;
        }
        else
        {
            Status = QuizResultStatus.Completed;
            IsPassed = false;
        }
    }
}
