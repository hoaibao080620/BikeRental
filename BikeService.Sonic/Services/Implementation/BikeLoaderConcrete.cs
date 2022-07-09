using BikeService.Sonic.DAL;
using BikeService.Sonic.Dtos.Bike;
using BikeService.Sonic.Exceptions;
using BikeService.Sonic.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BikeService.Sonic.Services.Implementation;

public class BikeLoaderConcrete : IBikeLoaderAdapter
{
    private readonly IUnitOfWork _unitOfWork;

    public BikeLoaderConcrete(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<BikeRetrieveDto> GetBike(int bikeId)
    {
        var bike = await _unitOfWork.BikeRepository.Find(b => b.Id == bikeId);
        return await bike
            .AsNoTracking()
            .Select(b => new BikeRetrieveDto
            {
                BikeStationId = b.BikeStationId,
                BikeStationName = b.BikeStation != null ? b.BikeStation.Name : null,
                Id = b.Id,
                Description = b.Description,
                LicensePlate = b.LicensePlate,
                Status = b.Status,
                UpdatedOn = b.UpdatedOn
            }).FirstOrDefaultAsync() ?? throw new BikeNotFoundException(bikeId);
    }

    public async Task<List<int>> GetBikeIdsOfManager(string managerEmail)
    {
        var bikeIds = (await _unitOfWork.BikeStationManagerRepository
                .Find(x => x.Manager.Email == managerEmail || x.Manager.IsSuperManager))
            .SelectMany(x => x.BikeStation.Bikes)
            .AsNoTracking()
            .Select(b => b.Id);

        return bikeIds.ToList();
    }
}
