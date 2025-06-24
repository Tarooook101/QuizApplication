using QuizApp.Domain.Common;


namespace QuizApp.Domain.Events.Category;

public sealed class CategoryActivatedEvent : BaseDomainEvent
{
    public Guid CategoryId { get; }
    public string Name { get; }

    public CategoryActivatedEvent(Guid categoryId, string name)
    {
        CategoryId = categoryId;
        Name = name;
        OccurredOn = DateTime.UtcNow;
    }
}
