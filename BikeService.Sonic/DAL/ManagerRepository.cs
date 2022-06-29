using BikeService.Sonic.BikeDbContext;
using BikeService.Sonic.Models;
using Shared.Repositories;

namespace BikeService.Sonic.DAL;

public class ManagerRepository : RepositoryGeneric<Manager, BikeServiceDbContext>, IManagerRepository
{
    public ManagerRepository(BikeServiceDbContext context) : base(context)
    {
    }
}
