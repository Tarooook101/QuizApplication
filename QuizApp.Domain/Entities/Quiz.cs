using QuizApp.Domain.Common;
using QuizApp.Domain.Enums;
using QuizApp.Domain.Events.QuizEvents;

namespace QuizApp.Domain.Entities;

public class Quiz : BaseEntity<Guid>
{
    public string Title { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public string? Instructions { get; private set; }
    public int TimeLimit { get; private set; }
    public int MaxAttempts { get; private set; }
    public bool IsPublic { get; private set; }
    public bool IsActive { get; private set; }
    public QuizDifficulty Difficulty { get; private set; }
    public string? ThumbnailUrl { get; private set; }
    public string? Tags { get; private set; }
    public Guid CreatedByUserId { get; private set; }

    private Quiz() : base() { }

    public Quiz(
        string title,
        string description,
        int timeLimit,
        int maxAttempts,
        QuizDifficulty difficulty,
        Guid createdByUserId,
        string? instructions = null,
        bool isPublic = true,
        string? thumbnailUrl = null,
        string? tags = null) : base(Guid.NewGuid())
    {
        SetTitle(title);
        SetDescription(description);
        SetInstructions(instructions);
        SetTimeLimit(timeLimit);
        SetMaxAttempts(maxAttempts);
        SetDifficulty(difficulty);
        SetCreatedByUserId(createdByUserId);
        SetIsPublic(isPublic);
        SetThumbnailUrl(thumbnailUrl);
        SetTags(tags);
        IsActive = true;

        AddDomainEvent(new QuizCreatedEvent(this));
    }

    public void Update(
        string title,
        string description,
        string? instructions,
        int timeLimit,
        int maxAttempts,
        QuizDifficulty difficulty,
        bool isPublic,
        string? thumbnailUrl,
        string? tags,
        string? updatedBy = null)
    {
        SetTitle(title);
        SetDescription(description);
        SetInstructions(instructions);
        SetTimeLimit(timeLimit);
        SetMaxAttempts(maxAttempts);
        SetDifficulty(difficulty);
        SetIsPublic(isPublic);
        SetThumbnailUrl(thumbnailUrl);
        SetTags(tags);
        MarkAsUpdated(updatedBy);

        AddDomainEvent(new QuizUpdatedEvent(this));
    }

    public void Activate(string? updatedBy = null)
    {
        IsActive = true;
        MarkAsUpdated(updatedBy);
        AddDomainEvent(new QuizActivatedEvent(this));
    }

    public void Deactivate(string? updatedBy = null)
    {
        IsActive = false;
        MarkAsUpdated(updatedBy);
        AddDomainEvent(new QuizDeactivatedEvent(this));
    }

    private void SetTitle(string title)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Quiz title cannot be empty", nameof(title));

        if (title.Length > 200)
            throw new ArgumentException("Quiz title cannot exceed 200 characters", nameof(title));

        Title = title.Trim();
    }

    private void SetDescription(string description)
    {
        if (string.IsNullOrWhiteSpace(description))
            throw new ArgumentException("Quiz description cannot be empty", nameof(description));

        if (description.Length > 2000)
            throw new ArgumentException("Quiz description cannot exceed 2000 characters", nameof(description));

        Description = description.Trim();
    }

    private void SetInstructions(string? instructions)
    {
        if (!string.IsNullOrEmpty(instructions) && instructions.Length > 1000)
            throw new ArgumentException("Quiz instructions cannot exceed 1000 characters", nameof(instructions));

        Instructions = instructions?.Trim();
    }

    private void SetTimeLimit(int timeLimit)
    {
        if (timeLimit < 1)
            throw new ArgumentException("Time limit must be at least 1 minute", nameof(timeLimit));

        if (timeLimit > 480)
            throw new ArgumentException("Time limit cannot exceed 480 minutes (8 hours)", nameof(timeLimit));

        TimeLimit = timeLimit;
    }

    private void SetMaxAttempts(int maxAttempts)
    {
        if (maxAttempts < 1)
            throw new ArgumentException("Max attempts must be at least 1", nameof(maxAttempts));

        if (maxAttempts > 10)
            throw new ArgumentException("Max attempts cannot exceed 10", nameof(maxAttempts));

        MaxAttempts = maxAttempts;
    }

    private void SetDifficulty(QuizDifficulty difficulty)
    {
        if (!Enum.IsDefined(typeof(QuizDifficulty), difficulty))
            throw new ArgumentException("Invalid quiz difficulty", nameof(difficulty));

        Difficulty = difficulty;
    }

    private void SetCreatedByUserId(Guid createdByUserId)
    {
        if (createdByUserId == Guid.Empty)
            throw new ArgumentException("Created by user ID cannot be empty", nameof(createdByUserId));

        CreatedByUserId = createdByUserId;
    }

    private void SetIsPublic(bool isPublic)
    {
        IsPublic = isPublic;
    }

    private void SetThumbnailUrl(string? thumbnailUrl)
    {
        if (!string.IsNullOrEmpty(thumbnailUrl) && thumbnailUrl.Length > 500)
            throw new ArgumentException("Thumbnail URL cannot exceed 500 characters", nameof(thumbnailUrl));

        ThumbnailUrl = thumbnailUrl?.Trim();
    }

    private void SetTags(string? tags)
    {
        if (!string.IsNullOrEmpty(tags) && tags.Length > 500)
            throw new ArgumentException("Tags cannot exceed 500 characters", nameof(tags));

        Tags = tags?.Trim();
    }
}
