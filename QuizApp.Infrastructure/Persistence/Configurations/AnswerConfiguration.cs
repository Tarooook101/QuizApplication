using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using QuizApp.Domain.Entities;


namespace QuizApp.Infrastructure.Persistence.Configurations;

public class AnswerConfiguration : IEntityTypeConfiguration<Answer>
{
    public void Configure(EntityTypeBuilder<Answer> builder)
    {
        builder.HasKey(a => a.Id);

        builder.Property(a => a.Id)
            .ValueGeneratedNever();

        builder.Property(a => a.Text)
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(a => a.IsCorrect)
            .IsRequired();

        builder.Property(a => a.OrderIndex)
            .IsRequired();

        builder.Property(a => a.Explanation)
            .HasMaxLength(1000);

        builder.Property(a => a.QuestionId)
            .IsRequired();

        builder.Property(a => a.CreatedAt)
            .IsRequired();

        builder.Property(a => a.UpdatedAt);

        builder.Property(a => a.CreatedBy)
            .HasMaxLength(100);

        builder.Property(a => a.UpdatedBy)
            .HasMaxLength(100);

        builder.HasOne(a => a.Question)
            .WithMany()
            .HasForeignKey(a => a.QuestionId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(a => a.QuestionId);
        builder.HasIndex(a => new { a.QuestionId, a.OrderIndex });
        builder.HasIndex(a => a.CreatedAt);
    }
}