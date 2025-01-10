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
    public class QuizTagConfiguration : IEntityTypeConfiguration<QuizTag>
    {
        public void Configure(EntityTypeBuilder<QuizTag> builder)
        {
            builder.ToTable("QuizTags");

            builder.Property(qt => qt.Name)
                .HasMaxLength(50)
                .IsRequired();

            builder.Property(qt => qt.Description)
                .HasMaxLength(200);

            builder.HasMany(qt => qt.Quizzes)
                .WithMany(q => q.Tags)
                .UsingEntity(j => j.ToTable("QuizTagMappings"));

            builder.HasIndex(qt => qt.Name)
                .IsUnique();
        }
    }
}
