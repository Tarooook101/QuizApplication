using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using QuizApp.Domain.Entities;


namespace QuizApp.Infrastructure.Persistence.Configurations;

public class CategoryConfiguration : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.ToTable("Categories");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.Name)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(c => c.Description)
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(c => c.IconUrl)
            .HasMaxLength(500);

        builder.Property(c => c.Color)
            .HasMaxLength(7)
            .HasDefaultValue("#6366f1");

        builder.Property(c => c.IsActive)
            .HasDefaultValue(true);

        builder.Property(c => c.DisplayOrder)
            .HasDefaultValue(0);

        builder.Property(c => c.CreatedAt)
            .IsRequired();

        builder.Property(c => c.UpdatedAt);

        builder.Property(c => c.CreatedBy)
            .HasMaxLength(100);

        builder.Property(c => c.UpdatedBy)
            .HasMaxLength(100);

        builder.HasIndex(c => c.Name)
            .IsUnique();

        builder.HasIndex(c => c.IsActive);

        builder.HasIndex(c => c.DisplayOrder);

        builder.HasIndex(c => c.CreatedAt);
    }
}