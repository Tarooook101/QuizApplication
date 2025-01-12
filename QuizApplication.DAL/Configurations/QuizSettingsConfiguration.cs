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
    public class QuizSettingsConfiguration : IEntityTypeConfiguration<QuizSettings>
    {
        public void Configure(EntityTypeBuilder<QuizSettings> builder)
        {
            builder.ToTable("QuizSettings");

            builder.Property(qs => qs.CustomSettings)
                .HasColumnType("nvarchar(max)")
                .HasConversion(JsonValueConverter.DictionaryStringConverter)
                .Metadata.SetValueComparer(JsonValueConverter.DictionaryComparer);

            builder.HasOne(qs => qs.Quiz)
                .WithOne(q => q.Settings)
                .HasForeignKey<QuizSettings>(qs => qs.QuizId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }

}
