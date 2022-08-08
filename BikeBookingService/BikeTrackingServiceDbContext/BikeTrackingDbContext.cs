using BikeBookingService.Models;
using Microsoft.EntityFrameworkCore;

namespace BikeBookingService.BikeTrackingServiceDbContext;

public class BikeTrackingDbContext : DbContext
{
    public BikeTrackingDbContext(DbContextOptions<BikeTrackingDbContext> options) : base(options)
    {
        
    }

    public DbSet<Account> Account { get; set; } = null!;
    public DbSet<Bike> Bike { get; set; } = null!;
    public DbSet<BikeLocationTracking> BikeLocationTracking { get; set; } = null!;
    public DbSet<BikeLocationTrackingHistory> BikeLocationTrackingHistory { get; set; } = null!;
    public DbSet<BikeRentalBooking> BikeRentalBooking { get; set; } = null!;
    public DbSet<RentingPoint> RentingPoint { get; set; } = null!;
    public DbSet<RentingPointHistory> RentingPointHistory { get; set; } = null!;
}
