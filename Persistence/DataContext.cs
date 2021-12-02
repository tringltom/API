using Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Persistence;

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
    }
}

