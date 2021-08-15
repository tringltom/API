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

        public virtual DbSet<Value> Values { get; set; }
        public virtual DbSet<RefreshToken> RefreshTokens { get; set; }

        public virtual DbSet<ActivityType> ActivityTypes { get; set; }
        public virtual DbSet<PendingActivity> PendingActivities { get; set; }
        public virtual DbSet<Activity> Activities { get; set; }
        public virtual DbSet<ActivityMedia> ActivityMedia { get; set; }
        public virtual DbSet<PendingActivityMedia> PendingActivityMedia { get; set; }


        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Value>()
            .HasData(
                new Value { Id = 1, Name = "Value 101" },
                new Value { Id = 2, Name = "Value 102" },
                new Value { Id = 3, Name = "Value 103" },
                new Value { Id = 4, Name = "Value 104" },
                new Value { Id = 5, Name = "Value 105" },
                new Value { Id = 6, Name = "Value 106" }
            );

            builder.Entity<ActivityType>()
               .HasData(
                   new ActivityType { ID = 1, Name = "GoodDeed" },
                   new ActivityType { ID = 2, Name = "Joke" },
                   new ActivityType { ID = 3, Name = "Quote" },
                   new ActivityType { ID = 4, Name = "Puzzle" },
                   new ActivityType { ID = 5, Name = "Happening" },
                   new ActivityType { ID = 6, Name = "Challenge" }
               );
        }
    }
}
