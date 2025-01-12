using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using QuizApplication.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QuizApplication.DAL.Common;

namespace QuizApplication.DAL.Configurations
{
    public class AchievementConfiguration : IEntityTypeConfiguration<Achievement>
    {
        public void Configure(EntityTypeBuilder<Achievement> builder)
        {
            builder.ToTable("Achievements");

            builder.Property(a => a.Name)
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(a => a.Description)
                .HasMaxLength(500)
                .IsRequired();

            builder.Property(a => a.IconUrl)
                .HasMaxLength(500)
                .IsRequired();

            builder.Property(a => a.Criteria)
                .HasColumnType("nvarchar(max)")
                .HasConversion(JsonValueConverter.DictionaryStringConverter)
                .Metadata.SetValueComparer(JsonValueConverter.DictionaryComparer);

            builder.HasMany(a => a.UserAchievements)
                .WithOne(ua => ua.Achievement)
                .HasForeignKey(ua => ua.AchievementId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(a => a.Name)
                .IsUnique();
            builder.HasIndex(a => a.Type);
        }
    }
}
