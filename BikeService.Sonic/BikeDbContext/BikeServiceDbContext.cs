using BikeService.Sonic.Models;
using Microsoft.EntityFrameworkCore;

namespace BikeService.Sonic.BikeDbContext;

public class BikeServiceDbContext : DbContext
{
    public BikeServiceDbContext(DbContextOptions<BikeServiceDbContext> options) : base(options)
    {
        
    }

    public DbSet<Bike> Bike { get; set; } = null!;
    public DbSet<BikeStation> BikeStation { get; set; } = null!;
    public DbSet<Manager> Manager { get; set; } = null!;
    public DbSet<BikeStationManager> BikeStationManager { get; set; } = null!;
    public DbSet<BikeReport> BikeReport { get; set; } = null!;
}
