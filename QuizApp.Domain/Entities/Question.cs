using QuizApp.Domain.Common;
using QuizApp.Domain.Enums;
using QuizApp.Domain.Events.QuestionEvents;


namespace QuizApp.Domain.Entities;

public class Question : BaseEntity<Guid>
{
    public string Text { get; private set; } = string.Empty;
    public QuestionType Type { get; private set; }
    public int Points { get; private set; }
    public int OrderIndex { get; private set; }
    public bool IsRequired { get; private set; }
    public string? Explanation { get; private set; }
    public string? ImageUrl { get; private set; }
    public Guid QuizId { get; private set; }
    public Quiz Quiz { get; private set; } = null!;

    private Question() : base() { }

    public Question(
        string text,
        QuestionType type,
        int points,
        int orderIndex,
        Guid quizId,
        bool isRequired = true,
        string? explanation = null,
        string? imageUrl = null) : base(Guid.NewGuid())
    {
        SetText(text);
        SetType(type);
        SetPoints(points);
        SetOrderIndex(orderIndex);
        SetQuizId(quizId);
        SetIsRequired(isRequired);
        SetExplanation(explanation);
        SetImageUrl(imageUrl);

        AddDomainEvent(new QuestionCreatedEvent(this));
    }

    public void Update(
        string text,
        QuestionType type,
        int points,
        int orderIndex,
        bool isRequired,
        string? explanation,
        string? imageUrl,
        string? updatedBy = null)
    {
        SetText(text);
        SetType(type);
        SetPoints(points);
        SetOrderIndex(orderIndex);
        SetIsRequired(isRequired);
        SetExplanation(explanation);
        SetImageUrl(imageUrl);
        MarkAsUpdated(updatedBy);

        AddDomainEvent(new QuestionUpdatedEvent(this));
    }

    public void UpdateOrder(int orderIndex, string? updatedBy = null)
    {
        SetOrderIndex(orderIndex);
        MarkAsUpdated(updatedBy);
    }

    private void SetText(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
            throw new ArgumentException("Question text cannot be empty", nameof(text));

        if (text.Length > 1000)
            throw new ArgumentException("Question text cannot exceed 1000 characters", nameof(text));

        Text = text.Trim();
    }

    private void SetType(QuestionType type)
    {
        if (!Enum.IsDefined(typeof(QuestionType), type))
            throw new ArgumentException("Invalid question type", nameof(type));

        Type = type;
    }

    private void SetPoints(int points)
    {
        if (points < 1)
            throw new ArgumentException("Points must be at least 1", nameof(points));

        if (points > 100)
            throw new ArgumentException("Points cannot exceed 100", nameof(points));

        Points = points;
    }

    private void SetOrderIndex(int orderIndex)
    {
        if (orderIndex < 0)
            throw new ArgumentException("Order index cannot be negative", nameof(orderIndex));

        OrderIndex = orderIndex;
    }

    private void SetQuizId(Guid quizId)
    {
        if (quizId == Guid.Empty)
            throw new ArgumentException("Quiz ID cannot be empty", nameof(quizId));

        QuizId = quizId;
    }

    private void SetIsRequired(bool isRequired)
    {
        IsRequired = isRequired;
    }

    private void SetExplanation(string? explanation)
    {
        if (!string.IsNullOrEmpty(explanation) && explanation.Length > 2000)
            throw new ArgumentException("Explanation cannot exceed 2000 characters", nameof(explanation));

        Explanation = explanation?.Trim();
    }

    private void SetImageUrl(string? imageUrl)
    {
        if (!string.IsNullOrEmpty(imageUrl) && imageUrl.Length > 500)
            throw new ArgumentException("Image URL cannot exceed 500 characters", nameof(imageUrl));

        ImageUrl = imageUrl?.Trim();
    }
}