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
    public class QuestionConfiguration : IEntityTypeConfiguration<Question>
    {
        public void Configure(EntityTypeBuilder<Question> builder)
        {
            builder.ToTable("Questions");

            builder.Property(q => q.Text)
                .HasMaxLength(1000)
                .IsRequired();

            builder.Property(q => q.ImageUrl)
                .HasMaxLength(500);

            builder.Property(q => q.Explanation)
                .HasMaxLength(2000);

            builder.HasMany(q => q.Options)
                .WithOne(o => o.Question)
                .HasForeignKey(o => o.QuestionId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(q => q.QuizId);
            builder.HasIndex(q => q.Type);
        }
    }
}
