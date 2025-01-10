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
    public class OptionConfiguration : IEntityTypeConfiguration<Option>
    {
        public void Configure(EntityTypeBuilder<Option> builder)
        {
            builder.ToTable("Options");

            builder.Property(o => o.Text)
                .HasMaxLength(500)
                .IsRequired();

            builder.Property(o => o.Explanation)
                .HasMaxLength(1000);

            builder.HasIndex(o => o.QuestionId);
        }
    }

}
