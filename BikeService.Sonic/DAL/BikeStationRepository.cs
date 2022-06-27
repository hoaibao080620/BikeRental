using BikeService.Sonic.BikeDbContext;
using BikeService.Sonic.Models;
using Microsoft.EntityFrameworkCore;
using Shared.Repositories;

namespace BikeService.Sonic.DAL;

public class BikeStationRepository : RepositoryGeneric<BikeStation, BikeServiceDbContext>, IBikeStationRepository
{
    public BikeStationRepository(BikeServiceDbContext context) : base(context)
    {
    }

    public async Task<BikeStation?> GetBikeStationByName(string name)
    {
        return await Context.BikeStation.FirstOrDefaultAsync(b => b.Name.ToLower() == name.ToLower());
    }
}
