using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using QuizApp.Domain.Entities;


namespace QuizApp.Infrastructure.Persistence.Configurations;

public class QuizReviewConfiguration : IEntityTypeConfiguration<QuizReview>
{
    public void Configure(EntityTypeBuilder<QuizReview> builder)
    {
        builder.ToTable("QuizReviews");

        builder.HasKey(r => r.Id);

        builder.Property(r => r.Id)
            .ValueGeneratedNever();

        builder.Property(r => r.QuizId)
            .IsRequired();

        builder.Property(r => r.UserId)
            .IsRequired();

        builder.Property(r => r.Rating)
            .IsRequired();

        builder.Property(r => r.Comment)
            .HasMaxLength(1000);

        builder.Property(r => r.IsRecommended)
            .HasDefaultValue(true);

        builder.Property(r => r.IsPublic)
            .HasDefaultValue(true);

        builder.Property(r => r.ReviewDate)
            .IsRequired();

        builder.Property(r => r.CreatedAt)
            .IsRequired();

        builder.Property(r => r.UpdatedAt);

        builder.Property(r => r.CreatedBy)
            .HasMaxLength(256);

        builder.Property(r => r.UpdatedBy)
            .HasMaxLength(256);

        builder.HasIndex(r => r.QuizId);
        builder.HasIndex(r => r.UserId);
        builder.HasIndex(r => new { r.UserId, r.QuizId })
            .IsUnique();
        builder.HasIndex(r => r.Rating);
        builder.HasIndex(r => r.IsPublic);
        builder.HasIndex(r => r.CreatedAt);
    }
}