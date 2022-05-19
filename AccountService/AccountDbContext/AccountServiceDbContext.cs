using AccountService.Models;
using Microsoft.EntityFrameworkCore;

namespace AccountService.AccountDbContext;

public class AccountServiceDbContext : DbContext
{
    public AccountServiceDbContext(DbContextOptions<AccountServiceDbContext> options) : base(options)
    {
        
    }

    public DbSet<User> Users { get; set; } = null!;
    public DbSet<Account> Accounts { get; set; } = null!;
    public DbSet<AccountPoint> AccountPoints { get; set; } = null!;
    public DbSet<AccountTransaction> AccountTransactions { get; set; } = null!;
    public DbSet<TransactionType> TransactionTypes { get; set; } = null!;
    public DbSet<PaymentGateway> PaymentGateways { get; set; } = null!;
}