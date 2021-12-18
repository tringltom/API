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
        public virtual DbSet<XpLevel> XpLevel { get; set; }

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
                .Entity<ActivityType>().HasData(
                    Enum.GetValues(typeof(ActivityTypeId))
                        .Cast<ActivityTypeId>()
                        .Select(e => new ActivityType()
                        {
                            Id = e,
                            Name = e.ToString()
                        })
                );


            builder.Entity<XpLevel>().HasData(
                        new XpLevel
                        {
                            Id = 1,
                            Xp = 0,
                            Level = 1
                        },
                        new XpLevel
                        {
                            Id = 2,
                            Xp = 1000,
                            Level = 2
                        },
                        new XpLevel
                        {
                            Id = 3,
                            Xp = 3000,
                            Level = 3
                        },
                        new XpLevel
                        {
                            Id = 4,
                            Xp = 6000,
                            Level = 4
                        },
                        new XpLevel
                        {
                            Id = 5,
                            Xp = 10000,
                            Level = 5
                        },
                        new XpLevel
                        {
                            Id = 6,
                            Xp = 15000,
                            Level = 6
                        },
                        new XpLevel
                        {
                            Id = 7,
                            Xp = 21000,
                            Level = 7
                        },
                        new XpLevel
                        {
                            Id = 8,
                            Xp = 28000,
                            Level = 8
                        },
                        new XpLevel
                        {
                            Id = 9,
                            Xp = 36000,
                            Level = 9
                        },
                        new XpLevel
                        {
                            Id = 10,
                            Xp = 45000,
                            Level = 10
                        },
                        new XpLevel
                        {
                            Id = 11,
                            Xp = 55000,
                            Level = 11
                        },
                        new XpLevel
                        {
                            Id = 12,
                            Xp = 66000,
                            Level = 12
                        },
                        new XpLevel
                        {
                            Id = 13,
                            Xp = 78000,
                            Level = 13
                        },
                        new XpLevel
                        {
                            Id = 14,
                            Xp = 91000,
                            Level = 14
                        },
                        new XpLevel
                        {
                            Id = 15,
                            Xp = 105000,
                            Level = 15
                        },
                        new XpLevel
                        {
                            Id = 16,
                            Xp = 120000,
                            Level = 16
                        },
                        new XpLevel
                        {
                            Id = 17,
                            Xp = 136000,
                            Level = 17
                        },
                        new XpLevel
                        {
                            Id = 18,
                            Xp = 153000,
                            Level = 18
                        },
                        new XpLevel
                        {
                            Id = 19,
                            Xp = 171000,
                            Level = 19
                        },
                        new XpLevel
                        {
                            Id = 20,
                            Xp = 190000,
                            Level = 20
                        });
        }
    }
}
