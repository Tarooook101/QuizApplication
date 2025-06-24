using QuizApp.Domain.Common;
using QuizApp.Domain.Events.AnswerEvents;

namespace QuizApp.Domain.Entities;

public class Answer : BaseEntity<Guid>
{
    public string Text { get; private set; } = string.Empty;
    public bool IsCorrect { get; private set; }
    public int OrderIndex { get; private set; }
    public string? Explanation { get; private set; }
    public Guid QuestionId { get; private set; }
    public Question Question { get; private set; } = null!;

    private Answer() : base() { }

    public Answer(
        string text,
        bool isCorrect,
        int orderIndex,
        Guid questionId,
        string? explanation = null) : base(Guid.NewGuid())
    {
        SetText(text);
        SetIsCorrect(isCorrect);
        SetOrderIndex(orderIndex);
        SetQuestionId(questionId);
        SetExplanation(explanation);

        AddDomainEvent(new AnswerCreatedEvent(this));
    }

    public void Update(
        string text,
        bool isCorrect,
        int orderIndex,
        string? explanation,
        string? updatedBy = null)
    {
        SetText(text);
        SetIsCorrect(isCorrect);
        SetOrderIndex(orderIndex);
        SetExplanation(explanation);
        MarkAsUpdated(updatedBy);

        AddDomainEvent(new AnswerUpdatedEvent(this));
    }

    public void UpdateOrder(int orderIndex, string? updatedBy = null)
    {
        SetOrderIndex(orderIndex);
        MarkAsUpdated(updatedBy);
    }

    private void SetText(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
            throw new ArgumentException("Answer text cannot be empty", nameof(text));

        if (text.Length > 500)
            throw new ArgumentException("Answer text cannot exceed 500 characters", nameof(text));

        Text = text.Trim();
    }

    private void SetIsCorrect(bool isCorrect)
    {
        IsCorrect = isCorrect;
    }

    private void SetOrderIndex(int orderIndex)
    {
        if (orderIndex < 0)
            throw new ArgumentException("Order index cannot be negative", nameof(orderIndex));

        OrderIndex = orderIndex;
    }

    private void SetQuestionId(Guid questionId)
    {
        if (questionId == Guid.Empty)
            throw new ArgumentException("Question ID cannot be empty", nameof(questionId));

        QuestionId = questionId;
    }

    private void SetExplanation(string? explanation)
    {
        if (!string.IsNullOrEmpty(explanation) && explanation.Length > 1000)
            throw new ArgumentException("Explanation cannot exceed 1000 characters", nameof(explanation));

        Explanation = explanation?.Trim();
    }
}
