using AccountService.Models;
using Microsoft.EntityFrameworkCore;

namespace AccountService.AccountDbContext;

public class AccountServiceDbContext : DbContext
{
    public AccountServiceDbContext(DbContextOptions<AccountServiceDbContext> options) : base(options)
    {
        
    }

    public DbSet<User> User { get; set; } = null!;
    public DbSet<Account> Account { get; set; } = null!;
    public DbSet<AccountTransaction> AccountTransaction { get; set; } = null!;
    public DbSet<TransactionType> TransactionType { get; set; } = null!;
}
