using BikeService.Sonic.Models;
using Microsoft.EntityFrameworkCore;

namespace BikeService.Sonic.BikeDbContext;

public class BikeServiceDbContext : DbContext
{
    public BikeServiceDbContext(DbContextOptions<BikeServiceDbContext> options) : base(options)
    {
        
    }

    public DbSet<Account> Users { get; set; } = null!;
    public DbSet<Bike> Bikes { get; set; } = null!;
    public DbSet<BikeStation> BikeStations { get; set; } = null!;
    public DbSet<BikeRentalTracking> BikeRentalTrackings { get; set; } = null!;
    public DbSet<Manager> Managers { get; set; }
    public DbSet<BikeStationManager> BikeStationManagers { get; set; }
}