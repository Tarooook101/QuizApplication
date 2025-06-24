using QuizApp.Domain.Common;
using QuizApp.Domain.Events.UserAnswerEvents;


namespace QuizApp.Domain.Entities;

public class UserAnswer : BaseEntity<Guid>
{
    public Guid QuizAttemptId { get; private set; }
    public Guid QuestionId { get; private set; }
    public Guid? SelectedAnswerId { get; private set; }
    public string? TextAnswer { get; private set; }
    public bool IsCorrect { get; private set; }
    public int PointsEarned { get; private set; }
    public DateTime AnsweredAt { get; private set; }
    public TimeSpan TimeSpent { get; private set; }

    // Navigation properties
    public QuizAttempt QuizAttempt { get; private set; } = null!;
    public Question Question { get; private set; } = null!;
    public Answer? SelectedAnswer { get; private set; }

    private UserAnswer() : base() { }

    public UserAnswer(
        Guid quizAttemptId,
        Guid questionId,
        Guid? selectedAnswerId = null,
        string? textAnswer = null,
        TimeSpan? timeSpent = null) : base(Guid.NewGuid())
    {
        SetQuizAttemptId(quizAttemptId);
        SetQuestionId(questionId);
        SetSelectedAnswerId(selectedAnswerId);
        SetTextAnswer(textAnswer);
        SetTimeSpent(timeSpent ?? TimeSpan.Zero);
        AnsweredAt = DateTime.UtcNow;
        IsCorrect = false;
        PointsEarned = 0;

        AddDomainEvent(new UserAnswerCreatedEvent(this));
    }

    public void UpdateAnswer(
        Guid? selectedAnswerId = null,
        string? textAnswer = null,
        TimeSpan? timeSpent = null,
        string? updatedBy = null)
    {
        SetSelectedAnswerId(selectedAnswerId);
        SetTextAnswer(textAnswer);
        if (timeSpent.HasValue)
        {
            SetTimeSpent(timeSpent.Value);
        }
        AnsweredAt = DateTime.UtcNow;
        MarkAsUpdated(updatedBy);

        AddDomainEvent(new UserAnswerUpdatedEvent(this));
    }

    public void MarkAsCorrect(int pointsEarned, string? updatedBy = null)
    {
        if (pointsEarned < 0)
            throw new ArgumentException("Points earned cannot be negative", nameof(pointsEarned));

        IsCorrect = true;
        PointsEarned = pointsEarned;
        MarkAsUpdated(updatedBy);

        AddDomainEvent(new UserAnswerGradedEvent(this));
    }

    public void MarkAsIncorrect(string? updatedBy = null)
    {
        IsCorrect = false;
        PointsEarned = 0;
        MarkAsUpdated(updatedBy);

        AddDomainEvent(new UserAnswerGradedEvent(this));
    }

    private void SetQuizAttemptId(Guid quizAttemptId)
    {
        if (quizAttemptId == Guid.Empty)
            throw new ArgumentException("Quiz attempt ID cannot be empty", nameof(quizAttemptId));

        QuizAttemptId = quizAttemptId;
    }

    private void SetQuestionId(Guid questionId)
    {
        if (questionId == Guid.Empty)
            throw new ArgumentException("Question ID cannot be empty", nameof(questionId));

        QuestionId = questionId;
    }

    private void SetSelectedAnswerId(Guid? selectedAnswerId)
    {
        SelectedAnswerId = selectedAnswerId;
    }

    private void SetTextAnswer(string? textAnswer)
    {
        if (!string.IsNullOrEmpty(textAnswer) && textAnswer.Length > 2000)
            throw new ArgumentException("Text answer cannot exceed 2000 characters", nameof(textAnswer));

        TextAnswer = textAnswer?.Trim();
    }

    private void SetTimeSpent(TimeSpan timeSpent)
    {
        if (timeSpent < TimeSpan.Zero)
            throw new ArgumentException("Time spent cannot be negative", nameof(timeSpent));

        TimeSpent = timeSpent;
    }
}
