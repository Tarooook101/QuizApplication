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
    public class QuestionMetadataConfiguration : IEntityTypeConfiguration<QuestionMetadata>
    {
        public void Configure(EntityTypeBuilder<QuestionMetadata> builder)
        {
            builder.ToTable("QuestionMetadata");

            builder.Property(qm => qm.SourceReference)
                .HasMaxLength(500);

            builder.Property(qm => qm.TopicArea)
                .HasMaxLength(200);

            builder.Property(qm => qm.LearningObjective)
                .HasMaxLength(500);

            builder.Property(qm => qm.CustomMetadata)
                .HasColumnType("nvarchar(max)")
                .HasConversion(JsonValueConverter.DictionaryStringConverter)
                .Metadata.SetValueComparer(JsonValueConverter.DictionaryComparer);

            builder.HasOne(qm => qm.Question)
                .WithOne(q => q.Metadata)
                .HasForeignKey<QuestionMetadata>(qm => qm.QuestionId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
