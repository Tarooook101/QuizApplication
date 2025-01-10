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
    public class QuizConfiguration : IEntityTypeConfiguration<Quiz>
    {
        public void Configure(EntityTypeBuilder<Quiz> builder)
        {
            builder.ToTable("Quizzes");

            builder.Property(q => q.Title)
                .HasMaxLength(200)
                .IsRequired();

            builder.Property(q => q.Description)
                .HasMaxLength(2000)
                .IsRequired();

            builder.HasMany(q => q.Questions)
                .WithOne(q => q.Quiz)
                .HasForeignKey(q => q.QuizId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(q => q.Settings)
                .WithOne(s => s.Quiz)
                .HasForeignKey<QuizSettings>(s => s.QuizId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(q => q.Categories)
                .WithMany(c => c.Quizzes)
                .UsingEntity(j => j.ToTable("QuizCategories"));

            builder.HasIndex(q => q.Status);
            builder.HasIndex(q => q.StartDate);
        }
    }
}
