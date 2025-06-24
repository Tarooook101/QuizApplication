using QuizApp.Domain.Common;


namespace QuizApp.Domain.Events.Category;

public sealed class CategoryUpdatedEvent : BaseDomainEvent
{
    public Guid CategoryId { get; }
    public string Name { get; }

    public CategoryUpdatedEvent(Guid categoryId, string name)
    {
        CategoryId = categoryId;
        Name = name;
        OccurredOn = DateTime.UtcNow;
    }
}
