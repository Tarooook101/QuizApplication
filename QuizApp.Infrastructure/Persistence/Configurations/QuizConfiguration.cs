using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using QuizApp.Domain.Entities;


namespace QuizApp.Infrastructure.Persistence.Configurations;

public class QuizConfiguration : IEntityTypeConfiguration<Quiz>
{
    public void Configure(EntityTypeBuilder<Quiz> builder)
    {
        builder.HasKey(q => q.Id);

        builder.Property(q => q.Title)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(q => q.Description)
            .HasMaxLength(2000)
            .IsRequired();

        builder.Property(q => q.Instructions)
            .HasMaxLength(1000);

        builder.Property(q => q.TimeLimit)
            .IsRequired();

        builder.Property(q => q.MaxAttempts)
            .IsRequired();

        builder.Property(q => q.IsPublic)
            .HasDefaultValue(true);

        builder.Property(q => q.IsActive)
            .HasDefaultValue(true);

        builder.Property(q => q.Difficulty)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(q => q.ThumbnailUrl)
            .HasMaxLength(500);

        builder.Property(q => q.Tags)
            .HasMaxLength(500);

        builder.Property(q => q.CreatedByUserId)
            .IsRequired();

        builder.Property(q => q.CreatedAt)
            .IsRequired();

        builder.Property(q => q.UpdatedAt);

        builder.Property(q => q.CreatedBy)
            .HasMaxLength(256);

        builder.Property(q => q.UpdatedBy)
            .HasMaxLength(256);

        builder.HasIndex(q => q.Title);
        builder.HasIndex(q => q.IsPublic);
        builder.HasIndex(q => q.IsActive);
        builder.HasIndex(q => q.Difficulty);
        builder.HasIndex(q => q.CreatedByUserId);
        builder.HasIndex(q => q.CreatedAt);

        builder.ToTable("Quizzes");
    }
}