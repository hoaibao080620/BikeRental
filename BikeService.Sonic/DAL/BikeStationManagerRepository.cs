using BikeService.Sonic.BikeDbContext;
using BikeService.Sonic.Models;
using Microsoft.EntityFrameworkCore;
using Shared.Repositories;

namespace BikeService.Sonic.DAL;

public class BikeStationManagerRepository : RepositoryGeneric<BikeStationManager, BikeServiceDbContext>, IBikeStationManagerRepository
{
    public BikeStationManagerRepository(BikeServiceDbContext context) : base(context)
    {
    }

    public async Task<List<string>> GetManagerEmailsByBikeId(int bikeId)
    {
        var managerEmails = await Context.BikeStationManagers
            .Include(b => b.Manager)
            .Where(b => b.BikeStation.Bikes.Any(x => x.Id == bikeId))
            .Select(x => x.Manager.Email)
            .ToListAsync();

        return managerEmails!;
    }
}