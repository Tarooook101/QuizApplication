using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using QuizApp.Domain.Entities;

namespace QuizApp.Infrastructure.Persistence.Configurations;

public class QuizAttemptConfiguration : IEntityTypeConfiguration<QuizAttempt>
{
    public void Configure(EntityTypeBuilder<QuizAttempt> builder)
    {
        builder.HasKey(qa => qa.Id);

        builder.Property(qa => qa.Id)
            .ValueGeneratedNever();

        builder.Property(qa => qa.QuizId)
            .IsRequired();

        builder.Property(qa => qa.UserId)
            .IsRequired();

        builder.Property(qa => qa.StartedAt)
            .IsRequired();

        builder.Property(qa => qa.CompletedAt);

        builder.Property(qa => qa.Status)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(qa => qa.Score);

        builder.Property(qa => qa.MaxScore);

        builder.Property(qa => qa.Percentage)
            .HasPrecision(5, 2);

        builder.Property(qa => qa.TimeSpentMinutes)
            .HasDefaultValue(0);

        builder.Property(qa => qa.Notes)
            .HasMaxLength(1000);

        builder.Property(qa => qa.CreatedAt)
            .IsRequired();

        builder.Property(qa => qa.UpdatedAt);

        builder.Property(qa => qa.CreatedBy)
            .HasMaxLength(100);

        builder.Property(qa => qa.UpdatedBy)
            .HasMaxLength(100);

        builder.HasIndex(qa => qa.QuizId);
        builder.HasIndex(qa => qa.UserId);
        builder.HasIndex(qa => qa.Status);
        builder.HasIndex(qa => qa.StartedAt);
        builder.HasIndex(qa => new { qa.UserId, qa.QuizId });
    }
}