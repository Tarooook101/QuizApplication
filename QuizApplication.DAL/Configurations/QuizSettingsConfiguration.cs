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
    public class QuizSettingsConfiguration : IEntityTypeConfiguration<QuizSettings>
    {
        public void Configure(EntityTypeBuilder<QuizSettings> builder)
        {
            builder.ToTable("QuizSettings");

            builder.Property(qs => qs.CustomSettings)
                .HasColumnType("jsonb");  // Using JSON storage for the custom settings dictionary

            builder.HasOne(qs => qs.Quiz)
                .WithOne(q => q.Settings)
                .HasForeignKey<QuizSettings>(qs => qs.QuizId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }

}
