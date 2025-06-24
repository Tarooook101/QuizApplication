using QuizApp.Domain.Common;
using QuizApp.Domain.Events.Category;


namespace QuizApp.Domain.Entities;

public class Category : BaseEntity<Guid>
{
    public string Name { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public string? IconUrl { get; private set; }
    public bool IsActive { get; private set; } = true;
    public int DisplayOrder { get; private set; }
    public string Color { get; private set; } = "#6366f1";

    private Category() : base() { }

    public Category(string name, string description, string? iconUrl = null, int displayOrder = 0, string color = "#6366f1")
        : base(Guid.NewGuid())
    {
        Name = name;
        Description = description;
        IconUrl = iconUrl;
        DisplayOrder = displayOrder;
        Color = color;

        AddDomainEvent(new CategoryCreatedEvent(Id, Name));
    }

    public void UpdateDetails(string name, string description, string? iconUrl = null, string? updatedBy = null)
    {
        if (Name != name || Description != description || IconUrl != iconUrl)
        {
            Name = name;
            Description = description;
            IconUrl = iconUrl;

            MarkAsUpdated(updatedBy);
            AddDomainEvent(new CategoryUpdatedEvent(Id, Name));
        }
    }

    public void UpdateDisplayOrder(int displayOrder, string? updatedBy = null)
    {
        if (DisplayOrder != displayOrder)
        {
            DisplayOrder = displayOrder;
            MarkAsUpdated(updatedBy);
        }
    }

    public void UpdateColor(string color, string? updatedBy = null)
    {
        if (Color != color)
        {
            Color = color;
            MarkAsUpdated(updatedBy);
        }
    }

    public void Activate(string? updatedBy = null)
    {
        if (!IsActive)
        {
            IsActive = true;
            MarkAsUpdated(updatedBy);
            AddDomainEvent(new CategoryActivatedEvent(Id, Name));
        }
    }

    public void Deactivate(string? updatedBy = null)
    {
        if (IsActive)
        {
            IsActive = false;
            MarkAsUpdated(updatedBy);
            AddDomainEvent(new CategoryDeactivatedEvent(Id, Name));
        }
    }
}