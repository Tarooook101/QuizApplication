using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using QuizApp.Domain.Entities;


namespace QuizApp.Infrastructure.Persistence.Configurations;

public class QuizResultConfiguration : IEntityTypeConfiguration<QuizResult>
{
    public void Configure(EntityTypeBuilder<QuizResult> builder)
    {
        builder.ToTable("QuizResults");

        builder.HasKey(qr => qr.Id);

        builder.Property(qr => qr.Id)
            .ValueGeneratedNever();

        builder.Property(qr => qr.QuizAttemptId)
            .IsRequired();

        builder.Property(qr => qr.UserId)
            .IsRequired();

        builder.Property(qr => qr.QuizId)
            .IsRequired();

        builder.Property(qr => qr.Score)
            .IsRequired();

        builder.Property(qr => qr.MaxScore)
            .IsRequired();

        builder.Property(qr => qr.Percentage)
            .HasPrecision(5, 2)
            .IsRequired();

        builder.Property(qr => qr.CorrectAnswers)
            .IsRequired();

        builder.Property(qr => qr.TotalQuestions)
            .IsRequired();

        builder.Property(qr => qr.TimeSpent)
            .IsRequired();

        builder.Property(qr => qr.Status)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(qr => qr.Feedback)
            .HasMaxLength(2000);

        builder.Property(qr => qr.CompletedAt)
            .IsRequired();

        builder.Property(qr => qr.IsPassed)
            .IsRequired();

        builder.Property(qr => qr.PassingThreshold)
            .HasPrecision(5, 2);

        builder.Property(qr => qr.CreatedAt)
            .IsRequired();

        builder.Property(qr => qr.UpdatedAt);

        builder.Property(qr => qr.CreatedBy)
            .HasMaxLength(256);

        builder.Property(qr => qr.UpdatedBy)
            .HasMaxLength(256);

        builder.HasOne(qr => qr.User)
            .WithMany()
            .HasForeignKey(qr => qr.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(qr => qr.QuizAttempt)
            .WithMany()
            .HasForeignKey(qr => qr.QuizAttemptId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(qr => qr.QuizAttemptId)
            .IsUnique();

        builder.HasIndex(qr => qr.UserId);

        builder.HasIndex(qr => qr.QuizId);

        builder.HasIndex(qr => qr.Status);

        builder.HasIndex(qr => qr.CompletedAt);

        builder.HasIndex(qr => new { qr.QuizId, qr.Score })
            .HasDatabaseName("IX_QuizResults_Quiz_Score");
    }
}