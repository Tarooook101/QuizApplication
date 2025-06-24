using QuizApp.Domain.Common;


namespace QuizApp.Domain.Events.Category;

public sealed class CategoryCreatedEvent : BaseDomainEvent
{
    public Guid CategoryId { get; }
    public string Name { get; }

    public CategoryCreatedEvent(Guid categoryId, string name)
    {
        CategoryId = categoryId;
        Name = name;
        OccurredOn = DateTime.UtcNow;
    }
}
