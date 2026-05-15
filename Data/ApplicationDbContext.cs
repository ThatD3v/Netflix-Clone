using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NetflixClone.Models;


namespace NetflixClone.Data;

public class ApplicationDbContext : IdentityDbContext<User>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<User>().ToTable("Users");
        builder.Entity<IdentityRole>().ToTable("Roles");
        builder.Entity<IdentityUserRole<string>>().ToTable("UserRoles");
        builder.Entity<IdentityUserClaim<string>>().ToTable("UserClaims");
        builder.Entity<IdentityUserLogin<string>>().ToTable("UserLogins");
        builder.Entity<IdentityUserToken<string>>().ToTable("UserTokens");
        builder.Entity<IdentityRoleClaim<string>>().ToTable("RoleClaims");


   
        builder.Entity<User>()
            .HasIndex(u => u.Email)
            .IsUnique();

        builder.Entity<User>()
            .HasIndex(u => u.PhoneNumber)
            .IsUnique();

        builder.Entity<User>()
            .HasIndex(u => u.IsActive);


        builder.Entity<Content>(entity =>
        {
            entity.ToTable("Contents");
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Title);
            entity.Property(e => e.Title).HasMaxLength(200);
        });

        builder.Entity<Genre>(entity =>
        {
            entity.ToTable("Genres");
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Name).IsUnique();
        });

        builder.Entity<ContentGenre>(entity =>
        {
            entity.ToTable("ContentGenres");
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => new { e.ContentId, e.GenreId }).IsUnique();
        });

        builder.Entity<Cast>(entity =>
        {
            entity.ToTable("Casts");
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.ContentId);
        });

        builder.Entity<Season>(entity =>
        {
            entity.ToTable("Seasons");
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => new { e.ContentId, e.SeasonNumber }).IsUnique();
        });

        builder.Entity<Episode>(entity =>
        {
            entity.ToTable("Episodes");
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => new { e.SeasonId, e.EpisodeNumber }).IsUnique();
        });


        builder.Entity<ProfileWatchHistory>(entity =>
        {
            entity.ToTable("ProfileWatchHistories");
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => new { e.ProfileId, e.ContentId });
            entity.HasIndex(e => e.LastWatchedAt);

            entity.HasOne(e => e.Profile)
                .WithMany()
                .HasForeignKey(e => e.ProfileId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        builder.Entity<ProfileMyList>(entity =>
        {
            entity.ToTable("ProfileMyLists");
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => new { e.ProfileId, e.ContentId }).IsUnique();

            entity.HasOne(e => e.Profile)
                .WithMany()
                .HasForeignKey(e => e.ProfileId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }


    public DbSet<SubscriptionPlan> SubscriptionPlans { get; set; }
    public DbSet<UserSubscription> UserSubscriptions { get; set; }
    public DbSet<PaymentHistory> PaymentHistory { get; set; }
    public DbSet<Profile> Profiles { get; set; }
    public DbSet<Content> Contents { get; set; }
    public DbSet<Genre> Genres { get; set; }
    public DbSet<ContentGenre> ContentGenres { get; set; }
    public DbSet<Cast> Casts { get; set; }
    public DbSet<Season> Seasons { get; set; }
    public DbSet<Episode> Episodes { get; set; }
    public DbSet<ProfileWatchHistory> ProfileWatchHistories { get; set; }
    public DbSet<ProfileMyList> ProfileMyLists { get; set; }

}