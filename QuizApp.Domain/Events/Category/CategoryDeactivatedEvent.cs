using QuizApp.Domain.Common;

namespace QuizApp.Domain.Events.Category;

public sealed class CategoryDeactivatedEvent : BaseDomainEvent
{
    public Guid CategoryId { get; init; }
    public string Name { get; init; }

    public CategoryDeactivatedEvent(Guid categoryId, string name)
    {
        CategoryId = categoryId;
        Name = name;
        OccurredOn = DateTime.UtcNow;
    }
}


