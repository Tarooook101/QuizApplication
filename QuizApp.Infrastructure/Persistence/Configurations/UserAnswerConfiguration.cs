using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using QuizApp.Domain.Entities;


namespace QuizApp.Infrastructure.Persistence.Configurations;

public class UserAnswerConfiguration : IEntityTypeConfiguration<UserAnswer>
{
    public void Configure(EntityTypeBuilder<UserAnswer> builder)
    {
        builder.ToTable("UserAnswers");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .ValueGeneratedNever();

        builder.Property(x => x.TextAnswer)
            .HasMaxLength(2000);

        builder.Property(x => x.CreatedBy)
            .HasMaxLength(256);

        builder.Property(x => x.UpdatedBy)
            .HasMaxLength(256);

        builder.HasOne(x => x.QuizAttempt)
            .WithMany()
            .HasForeignKey(x => x.QuizAttemptId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Question)
            .WithMany()
            .HasForeignKey(x => x.QuestionId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.SelectedAnswer)
            .WithMany()
            .HasForeignKey(x => x.SelectedAnswerId)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired(false);

        builder.HasIndex(x => new { x.QuizAttemptId, x.QuestionId })
            .IsUnique()
            .HasDatabaseName("IX_UserAnswers_QuizAttemptId_QuestionId");

        builder.HasIndex(x => x.QuizAttemptId)
            .HasDatabaseName("IX_UserAnswers_QuizAttemptId");

        builder.HasIndex(x => x.QuestionId)
            .HasDatabaseName("IX_UserAnswers_QuestionId");

        builder.HasIndex(x => x.IsCorrect)
            .HasDatabaseName("IX_UserAnswers_IsCorrect");

        builder.HasIndex(x => x.AnsweredAt)
            .HasDatabaseName("IX_UserAnswers_AnsweredAt");
    }
}
