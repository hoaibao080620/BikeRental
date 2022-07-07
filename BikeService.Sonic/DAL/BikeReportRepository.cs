using BikeService.Sonic.BikeDbContext;
using BikeService.Sonic.Models;
using Shared.Repositories;

namespace BikeService.Sonic.DAL;

public class BikeReportRepository : RepositoryGeneric<BikeReport, BikeServiceDbContext>, IBikeReportRepository
{
    public BikeReportRepository(BikeServiceDbContext context) : base(context)
    {
    }
}
