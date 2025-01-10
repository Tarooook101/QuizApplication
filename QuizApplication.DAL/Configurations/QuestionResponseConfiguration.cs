using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using QuizApplication.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizApplication.DAL.Configurations
{
    public class QuestionResponseConfiguration : IEntityTypeConfiguration<QuestionResponse>
    {
        public void Configure(EntityTypeBuilder<QuestionResponse> builder)
        {
            builder.ToTable("QuestionResponses");

            builder.Property(qr => qr.Response)
                .HasMaxLength(2000);

            builder.HasOne(qr => qr.Question)
                .WithMany(q => q.Responses)
                .HasForeignKey(qr => qr.QuestionId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(qr => qr.QuizAttempt)
                .WithMany(qa => qa.Responses)
                .HasForeignKey(qr => qr.QuizAttemptId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(qr => new { qr.QuizAttemptId, qr.QuestionId })
                .IsUnique();
        }
    }
}
