using Microsoft.EntityFrameworkCore;
using NetflixClone.Models;
using System.Numerics;

namespace NetflixClone.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Profile> Profiles { get; set; }
    public DbSet<WatchHistory> WatchHistories { get; set; }
    public DbSet<Favorite> Favorites { get; set; }
    public DbSet<Plan> Plans { get; set; }
    public DbSet<Subscription> Subscriptions { get; set; }
    public DbSet<UserDevices> UserDevices { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Seed Plans
        modelBuilder.Entity<Plan>().HasData(
            new Plan { Id = 1, Name = "Mobile", Price = 2500, VideoQuality = "Fair", Resolution = "480p", SpatialAudio = false, MaxDevices = 1, MaxDownloadDevices = 1, AllowedDeviceTypes = "mobile,tablet", Description = "Mobile devices only" },
            new Plan { Id = 2, Name = "Basic", Price = 4000, VideoQuality = "Good", Resolution = "720p", SpatialAudio = false, MaxDevices = 1, MaxDownloadDevices = 1, AllowedDeviceTypes = "tv,computer,mobile,tablet", Description = "HD streaming on 1 device" },
            new Plan { Id = 3, Name = "Standard", Price = 6500, VideoQuality = "Great", Resolution = "1080p", SpatialAudio = false, MaxDevices = 2, MaxDownloadDevices = 2, AllowedDeviceTypes = "tv,computer,mobile,tablet", Description = "Full HD streaming, 2 devices" },
            new Plan { Id = 4, Name = "Premium", Price = 8500, VideoQuality = "Best", Resolution = "4K + HDR", SpatialAudio = true, MaxDevices = 4, MaxDownloadDevices = 6, AllowedDeviceTypes = "tv,computer,mobile,tablet", Description = "Ultra HD streaming with HDR and spatial audio" }
        );


    }
}