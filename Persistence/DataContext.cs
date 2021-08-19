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

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<ActivityType>()
               .HasData(
                   new ActivityType { Id = 1, Name = "GoodDeed" },
                   new ActivityType { Id = 2, Name = "Joke" },
                   new ActivityType { Id = 3, Name = "Quote" },
                   new ActivityType { Id = 4, Name = "Puzzle" },
                   new ActivityType { Id = 5, Name = "Happening" },
                   new ActivityType { Id = 6, Name = "Challenge" }
               );
        }
    }
}
