using Microsoft.EntityFrameworkCore;
using SmartHomeAPI.Models;

namespace SmartHomeAPI.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) {}

    public DbSet<User> Users => Set<User>();
    public DbSet<Room> Rooms => Set<Room>();
    public DbSet<DeviceType> DeviceTypes => Set<DeviceType>();
    public DbSet<Device> Devices => Set<Device>();
    public DbSet<DeviceConfiguration> DeviceConfigurations => Set<DeviceConfiguration>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
    public DbSet<Client> Clients => Set<Client>();
    public DbSet<Location> Locations => Set<Location>();
    public DbSet<UserLocationAccess> UserLocationAccesses => Set<UserLocationAccess>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // A room belongs to exactly one location. There's no navigation property on
        // Room, so the FK is configured explicitly; deleting a location removes its rooms.
        modelBuilder.Entity<Room>()
            .HasOne<Location>()
            .WithMany()
            .HasForeignKey(r => r.LocationId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}