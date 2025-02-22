﻿using Microsoft.EntityFrameworkCore.Metadata.Builders;
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
    public class UserProfileConfiguration : IEntityTypeConfiguration<UserProfile>
    {
        public void Configure(EntityTypeBuilder<UserProfile> builder)
        {
            builder.ToTable("UserProfiles");

            builder.Property(up => up.TimeZone)
                .HasMaxLength(50)
                .IsRequired();

            builder.Property(up => up.Language)
                .HasMaxLength(10)
                .IsRequired();

            builder.Property(up => up.Biography)
                .HasMaxLength(1000);

            builder.Property(up => up.CustomSettings)
                .HasColumnType("nvarchar(max)")
                .HasConversion(JsonValueConverter.DictionaryStringConverter)
                .Metadata.SetValueComparer(JsonValueConverter.DictionaryComparer);

            builder.Property(up => up.NotificationPreferences)
                .HasColumnType("nvarchar(max)")
                .HasConversion(JsonValueConverter.Create<NotificationPreferences>())
                .Metadata.SetValueComparer(JsonValueConverter.CreateComparer<NotificationPreferences>());

            builder.HasOne(up => up.User)
                .WithOne(u => u.Profile)
                .HasForeignKey<UserProfile>(up => up.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
