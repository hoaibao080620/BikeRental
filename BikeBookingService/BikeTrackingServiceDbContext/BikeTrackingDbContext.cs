using BikeBookingService.Models;
using Microsoft.EntityFrameworkCore;

namespace BikeBookingService.BikeTrackingServiceDbContext;

public class BikeTrackingDbContext : DbContext
{
    public BikeTrackingDbContext(DbContextOptions<BikeTrackingDbContext> options) : base(options)
    {
        
    }

    public DbSet<Account> Account { get; set; } = null!;
    public DbSet<Bike> Bikes { get; set; } = null!;
    public DbSet<BikeLocationTracking> BikeLocationTracking { get; set; } = null!;
    public DbSet<BikeLocationTrackingHistory> BikeLocationTrackingHistory { get; set; } = null!;
    public DbSet<BikeRentalBooking> BikeRentalBooking { get; set; } = null!;
}
