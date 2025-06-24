using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using QuizApp.Domain.Entities;


namespace QuizApp.Infrastructure.Persistence.Configurations;

public class QuestionConfiguration : IEntityTypeConfiguration<Question>
{
    public void Configure(EntityTypeBuilder<Question> builder)
    {
        builder.ToTable("Questions");

        builder.HasKey(q => q.Id);

        builder.Property(q => q.Id)
            .IsRequired()
            .ValueGeneratedNever();

        builder.Property(q => q.Text)
            .IsRequired()
            .HasMaxLength(1000);

        builder.Property(q => q.Type)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(q => q.Points)
            .IsRequired();

        builder.Property(q => q.OrderIndex)
            .IsRequired();

        builder.Property(q => q.IsRequired)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(q => q.Explanation)
            .HasMaxLength(2000);

        builder.Property(q => q.ImageUrl)
            .HasMaxLength(500);

        builder.Property(q => q.QuizId)
            .IsRequired();

        builder.Property(q => q.CreatedAt)
            .IsRequired();

        builder.Property(q => q.UpdatedAt);

        builder.Property(q => q.CreatedBy)
            .HasMaxLength(256);

        builder.Property(q => q.UpdatedBy)
            .HasMaxLength(256);

        builder.HasOne(q => q.Quiz)
            .WithMany()
            .HasForeignKey(q => q.QuizId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(q => q.QuizId);
        builder.HasIndex(q => new { q.QuizId, q.OrderIndex })
            .IsUnique();

        builder.Ignore(q => q.DomainEvents);
    }
}