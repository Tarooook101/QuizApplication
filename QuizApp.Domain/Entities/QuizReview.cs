using QuizApp.Domain.Common;
using QuizApp.Domain.Events.QuizReviewEvents;

namespace QuizApp.Domain.Entities;

public class QuizReview : BaseEntity<Guid>
{
    public Guid QuizId { get; private set; }
    public Guid UserId { get; private set; }
    public int Rating { get; private set; }
    public string? Comment { get; private set; }
    public bool IsRecommended { get; private set; }
    public bool IsPublic { get; private set; }
    public DateTime ReviewDate { get; private set; }

    private QuizReview() : base() { }

    public QuizReview(
        Guid quizId,
        Guid userId,
        int rating,
        string? comment = null,
        bool isRecommended = true,
        bool isPublic = true) : base(Guid.NewGuid())
    {
        SetQuizId(quizId);
        SetUserId(userId);
        SetRating(rating);
        SetComment(comment);
        SetIsRecommended(isRecommended);
        SetIsPublic(isPublic);
        ReviewDate = DateTime.UtcNow;

        AddDomainEvent(new Events.QuizReviewEvents.QuizReviewCreatedEvent(this));
    }

    public void Update(
        int rating,
        string? comment,
        bool isRecommended,
        bool isPublic,
        string? updatedBy = null)
    {
        SetRating(rating);
        SetComment(comment);
        SetIsRecommended(isRecommended);
        SetIsPublic(isPublic);
        MarkAsUpdated(updatedBy);

        AddDomainEvent(new QuizReviewUpdatedEvent(this));
    }

    public void MakePrivate(string? updatedBy = null)
    {
        IsPublic = false;
        MarkAsUpdated(updatedBy);
        AddDomainEvent(new QuizReviewMadePrivateEvent(this));
    }

    public void MakePublic(string? updatedBy = null)
    {
        IsPublic = true;
        MarkAsUpdated(updatedBy);
        AddDomainEvent(new QuizReviewMadePublicEvent(this));
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

    private void SetRating(int rating)
    {
        if (rating < 1 || rating > 5)
            throw new ArgumentException("Rating must be between 1 and 5", nameof(rating));

        Rating = rating;
    }

    private void SetComment(string? comment)
    {
        if (!string.IsNullOrEmpty(comment) && comment.Length > 1000)
            throw new ArgumentException("Comment cannot exceed 1000 characters", nameof(comment));

        Comment = comment?.Trim();
    }

    private void SetIsRecommended(bool isRecommended)
    {
        IsRecommended = isRecommended;
    }

    private void SetIsPublic(bool isPublic)
    {
        IsPublic = isPublic;
    }
}