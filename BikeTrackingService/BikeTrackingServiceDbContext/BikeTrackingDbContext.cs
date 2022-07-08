using BikeService.Sonic.Models;
using BikeTrackingService.Models;
using Microsoft.EntityFrameworkCore;

namespace BikeTrackingService.BikeTrackingServiceDbContext;

public class BikeTrackingDbContext : DbContext
{
    public BikeTrackingDbContext(DbContextOptions<BikeTrackingDbContext> options) : base(options)
    {
        
    }

    public DbSet<Account> User { get; set; } = null!;
    public DbSet<Bike> Bikes { get; set; } = null!;
    public DbSet<BikeLocationTracking> BikeLocationTracking { get; set; } = null!;
    public DbSet<BikeLocationTrackingHistory> BikeLocationTrackingHistory { get; set; } = null!;
    public DbSet<BikeRentalTracking> BikeRentalBooking { get; set; } = null!;
}
