using System;
using System.Linq;
using Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Persistence
{
    public class DataContext : IdentityDbContext<User, IdentityRole<int>, int>
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {

        }

        public virtual DbSet<RefreshToken> RefreshTokens { get; set; }
        public virtual DbSet<ActivityType> ActivityTypes { get; set; }
        public virtual DbSet<PendingActivity> PendingActivities { get; set; }
        public virtual DbSet<Activity> Activities { get; set; }
        public virtual DbSet<ActivityMedia> ActivityMedia { get; set; }
        public virtual DbSet<PendingActivityMedia> PendingActivityMedia { get; set; }
        public virtual DbSet<ActivityReviewXp> ActivityReviewXp { get; set; }
        public virtual DbSet<ReviewType> ReviewTypes { get; set; }
        public virtual DbSet<UserFavoriteActivity> UserFavoriteActivities { get; set; }
        public virtual DbSet<UserReview> UserReviews { get; set; }
        public virtual DbSet<XpLevel> XpLevels { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder
               .Entity<Activity>()
               .Property(e => e.ActivityTypeId)
               .HasConversion<int>();

            builder
               .Entity<PendingActivity>()
               .Property(e => e.ActivityTypeId)
               .HasConversion<int>();

            builder
                .Entity<ActivityType>()
                .Property(e => e.Id)
                .HasConversion<int>();

            builder
                .Entity<UserFavoriteActivity>()
                .HasIndex(uf => new { uf.UserId, uf.ActivityId })
                .IsUnique();

            builder
                .Entity<UserReview>()
                .HasIndex(ur => new { ur.UserId, ur.ActivityId })
                .IsUnique();



            builder
                .Entity<ActivityReviewXp>()
                .HasIndex(arx => new { arx.ReviewTypeId, arx.ActivityTypeId })
                .IsUnique();

            builder.Entity<ReviewType>()
                .HasIndex(rt => rt.Name).IsUnique();

            builder
                .Entity<ActivityType>().HasData(
                    Enum.GetValues(typeof(ActivityTypeId))
                        .Cast<ActivityTypeId>()
                        .Select(e => new ActivityType()
                        {
                            Id = e,
                            Name = e.ToString()
                        })
                );

            builder
            .Entity<User>()
            .Property(b => b.XpLevelId)
            .HasDefaultValue(1);

            builder.Entity<XpLevel>().HasData(
                        new XpLevel
                        {
                            Id = 1,
                            Xp = 0,
                        },
                        new XpLevel
                        {
                            Id = 2,
                            Xp = 1000,
                        },
                        new XpLevel
                        {
                            Id = 3,
                            Xp = 3000,
                        },
                        new XpLevel
                        {
                            Id = 4,
                            Xp = 6000,
                        },
                        new XpLevel
                        {
                            Id = 5,
                            Xp = 10000,
                        },
                        new XpLevel
                        {
                            Id = 6,
                            Xp = 15000,
                        },
                        new XpLevel
                        {
                            Id = 7,
                            Xp = 21000,
                        },
                        new XpLevel
                        {
                            Id = 8,
                            Xp = 28000,
                        },
                        new XpLevel
                        {
                            Id = 9,
                            Xp = 36000,
                        },
                        new XpLevel
                        {
                            Id = 10,
                            Xp = 45000,
                        },
                        new XpLevel
                        {
                            Id = 11,
                            Xp = 55000,
                        },
                        new XpLevel
                        {
                            Id = 12,
                            Xp = 66000,
                        },
                        new XpLevel
                        {
                            Id = 13,
                            Xp = 78000,
                        },
                        new XpLevel
                        {
                            Id = 14,
                            Xp = 91000,
                        },
                        new XpLevel
                        {
                            Id = 15,
                            Xp = 105000,
                        },
                        new XpLevel
                        {
                            Id = 16,
                            Xp = 120000,
                        },
                        new XpLevel
                        {
                            Id = 17,
                            Xp = 136000,
                        },
                        new XpLevel
                        {
                            Id = 18,
                            Xp = 153000,
                        },
                        new XpLevel
                        {
                            Id = 19,
                            Xp = 171000,
                        },
                        new XpLevel
                        {
                            Id = 20,
                            Xp = 190000,
                        }
                );

            builder
               .Entity<ReviewType>().HasData(
                   Enum.GetValues(typeof(ReviewTypeId))
                       .Cast<ReviewTypeId>()
                       .Select(e => new ReviewType()
                       {
                           Id = e,
                           Name = e
                       })
               );

            builder
                .Entity<ActivityReviewXp>().HasData(
                    new ActivityReviewXp { Id = 1, ActivityTypeId = ActivityTypeId.GoodDeed, ReviewTypeId = ReviewTypeId.None, Xp = 0 },
                    new ActivityReviewXp { Id = 2, ActivityTypeId = ActivityTypeId.GoodDeed, ReviewTypeId = ReviewTypeId.Poor, Xp = 20 },
                    new ActivityReviewXp { Id = 3, ActivityTypeId = ActivityTypeId.GoodDeed, ReviewTypeId = ReviewTypeId.Good, Xp = 50 },
                    new ActivityReviewXp { Id = 4, ActivityTypeId = ActivityTypeId.GoodDeed, ReviewTypeId = ReviewTypeId.Awesome, Xp = 200 },
                    new ActivityReviewXp { Id = 5, ActivityTypeId = ActivityTypeId.Joke, ReviewTypeId = ReviewTypeId.None, Xp = -10 },
                    new ActivityReviewXp { Id = 6, ActivityTypeId = ActivityTypeId.Joke, ReviewTypeId = ReviewTypeId.Poor, Xp = 0 },
                    new ActivityReviewXp { Id = 7, ActivityTypeId = ActivityTypeId.Joke, ReviewTypeId = ReviewTypeId.Good, Xp = 10 },
                    new ActivityReviewXp { Id = 8, ActivityTypeId = ActivityTypeId.Joke, ReviewTypeId = ReviewTypeId.Awesome, Xp = 40 },
                    new ActivityReviewXp { Id = 9, ActivityTypeId = ActivityTypeId.Quote, ReviewTypeId = ReviewTypeId.None, Xp = 0 },
                    new ActivityReviewXp { Id = 10, ActivityTypeId = ActivityTypeId.Quote, ReviewTypeId = ReviewTypeId.Poor, Xp = 0 },
                    new ActivityReviewXp { Id = 11, ActivityTypeId = ActivityTypeId.Quote, ReviewTypeId = ReviewTypeId.Good, Xp = 20 },
                    new ActivityReviewXp { Id = 12, ActivityTypeId = ActivityTypeId.Quote, ReviewTypeId = ReviewTypeId.Awesome, Xp = 50 },
                    new ActivityReviewXp { Id = 13, ActivityTypeId = ActivityTypeId.Puzzle, ReviewTypeId = ReviewTypeId.None, Xp = 0 },
                    new ActivityReviewXp { Id = 14, ActivityTypeId = ActivityTypeId.Puzzle, ReviewTypeId = ReviewTypeId.Poor, Xp = 0 },
                    new ActivityReviewXp { Id = 15, ActivityTypeId = ActivityTypeId.Puzzle, ReviewTypeId = ReviewTypeId.Good, Xp = 20 },
                    new ActivityReviewXp { Id = 16, ActivityTypeId = ActivityTypeId.Puzzle, ReviewTypeId = ReviewTypeId.Awesome, Xp = 40 },
                    new ActivityReviewXp { Id = 17, ActivityTypeId = ActivityTypeId.Happening, ReviewTypeId = ReviewTypeId.None, Xp = 0 },
                    new ActivityReviewXp { Id = 18, ActivityTypeId = ActivityTypeId.Happening, ReviewTypeId = ReviewTypeId.Poor, Xp = 10 },
                    new ActivityReviewXp { Id = 19, ActivityTypeId = ActivityTypeId.Happening, ReviewTypeId = ReviewTypeId.Good, Xp = 100 },
                    new ActivityReviewXp { Id = 20, ActivityTypeId = ActivityTypeId.Happening, ReviewTypeId = ReviewTypeId.Awesome, Xp = 200 },
                    new ActivityReviewXp { Id = 21, ActivityTypeId = ActivityTypeId.Challenge, ReviewTypeId = ReviewTypeId.None, Xp = 0 },
                    new ActivityReviewXp { Id = 22, ActivityTypeId = ActivityTypeId.Challenge, ReviewTypeId = ReviewTypeId.Poor, Xp = 10 },
                    new ActivityReviewXp { Id = 23, ActivityTypeId = ActivityTypeId.Challenge, ReviewTypeId = ReviewTypeId.Good, Xp = 100 },
                    new ActivityReviewXp { Id = 24, ActivityTypeId = ActivityTypeId.Challenge, ReviewTypeId = ReviewTypeId.Awesome, Xp = 250 }
                    );
        }
    }
}
