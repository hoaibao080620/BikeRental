using Microsoft.EntityFrameworkCore;
using UserService.Models;

namespace UserService.ApplicationDbContext;

public class UserServiceDbContext : DbContext
{
    public UserServiceDbContext(DbContextOptions<UserServiceDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; } = null!;
    public DbSet<Role> Roles { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Role>().HasData(new List<Role>
        {
            new()
            {
                Id = 1,
                Name = "User",
                CreatedOn = DateTime.UtcNow,
                IsActive = true
            },
            new()
            {
                Id = 2,
                Name = "Manager",
                CreatedOn = DateTime.UtcNow,
                IsActive = true
            },
            new()
            {
                Id = 3,
                Name = "SysAdmin",
                CreatedOn = DateTime.UtcNow,
                IsActive = true
            }
        });
    }
}