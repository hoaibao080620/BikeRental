using BikeService.Sonic.BikeDbContext;
using BikeService.Sonic.Models;
using Shared.Repositories;

namespace BikeService.Sonic.DAL;

public class BikeRepository : RepositoryGeneric<Bike, BikeServiceDbContext>, IBikeRepository
{
    public BikeRepository(BikeServiceDbContext context) : base(context)
    {
    }
}