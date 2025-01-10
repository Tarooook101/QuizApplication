using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.EntityFrameworkCore;
using QuizApplication.DAL.Common;
using QuizApplication.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using QuizApplication.DAL.Configurations;

namespace QuizApplication.DAL.Database
{

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Quiz> Quizzes => Set<Quiz>();
        public DbSet<Question> Questions => Set<Question>();
        public DbSet<Option> Options => Set<Option>();
        public DbSet<Category> Categories => Set<Category>();
        public DbSet<Achievement> Achievements => Set<Achievement>();
        public DbSet<QuestionMetadata> QuestionMetadata => Set<QuestionMetadata>();
        public DbSet<QuestionResponse> QuestionResponses => Set<QuestionResponse>();
        public DbSet<QuizAttempt> QuizAttempts => Set<QuizAttempt>();
        public DbSet<QuizSettings> QuizSettings => Set<QuizSettings>();
        public DbSet<QuizTag> QuizTags => Set<QuizTag>();
        public DbSet<UserAchievement> UserAchievements => Set<UserAchievement>();
        public DbSet<UserProfile> UserProfiles => Set<UserProfile>();

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Apply all configurations
            builder.ApplyConfiguration(new QuizConfiguration());
            builder.ApplyConfiguration(new QuestionConfiguration());
            builder.ApplyConfiguration(new OptionConfiguration());
            builder.ApplyConfiguration(new CategoryConfiguration());
            builder.ApplyConfiguration(new ApplicationUserConfiguration());
            builder.ApplyConfiguration(new AchievementConfiguration());
            builder.ApplyConfiguration(new QuestionMetadataConfiguration());
            builder.ApplyConfiguration(new QuestionResponseConfiguration());
            builder.ApplyConfiguration(new QuizAttemptConfiguration());
            builder.ApplyConfiguration(new QuizSettingsConfiguration());
            builder.ApplyConfiguration(new QuizTagConfiguration());
            builder.ApplyConfiguration(new UserAchievementConfiguration());
            builder.ApplyConfiguration(new UserProfileConfiguration());

            // Global query filters for soft delete
            builder.SetQueryFilterOnAllEntities<ISoftDeletable>(e => !e.IsDeleted);
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            UpdateAuditableEntities();
            return base.SaveChangesAsync(cancellationToken);
        }

        private void UpdateAuditableEntities()
        {
            var entries = ChangeTracker.Entries<IAuditableEntity>();

            foreach (var entry in entries)
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.Entity.CreatedAt = DateTimeOffset.UtcNow;
                        break;
                    case EntityState.Modified:
                        entry.Entity.LastModifiedAt = DateTimeOffset.UtcNow;
                        break;
                }
            }

        }

    }
}
