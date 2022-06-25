using BikeService.Sonic.DAL;
using BikeService.Sonic.Dtos.Bike;
using BikeService.Sonic.Mapper;
using BikeService.Sonic.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BikeService.Sonic.Services.Implementation;

public class BikeRepositoryConcrete : IBikeRepositoryAdapter
{
    private readonly IBikeRepository _bikeRepository;

    public BikeRepositoryConcrete(IBikeRepository bikeRepository)
    {
        _bikeRepository = bikeRepository;
    }

    public async Task<BikeRetrieveDto?> GetBike(int bikeId)
    {
        var bike = await _bikeRepository.Find(b => b.Id == bikeId);
        return bike.Include(b => b.BikeLocationTrackings).AsNoTracking()
            .Select(b => BikeMapper.Map(b)).FirstOrDefault();
    }

    public async Task<List<BikeRetrieveDto>> GetBikes(string managerEmail)
    {
        var bikes = await _bikeRepository.All();
        var bikesRetrieveDtos =  bikes.Where(b => 
            b.BikeStation != null && 
            b.BikeStation.BikeStationManagers.Any(bs => bs.Manager.Email == managerEmail)
        ).Include(b => b.BikeLocationTrackings).Select(b => BikeMapper.Map(b)).AsNoTracking().ToList();

        return bikesRetrieveDtos;
    }
}
