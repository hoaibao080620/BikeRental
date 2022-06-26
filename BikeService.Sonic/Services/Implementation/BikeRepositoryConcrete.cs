using BikeService.Sonic.DAL;
using BikeService.Sonic.Dtos.Bike;
using BikeService.Sonic.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BikeService.Sonic.Services.Implementation;

public class BikeRepositoryConcrete : IBikeRepositoryAdapter
{
    private readonly IUnitOfWork _unitOfWork;

    public BikeRepositoryConcrete(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<BikeRetrieveDto?> GetBike(int bikeId)
    {
        var bike = await _unitOfWork.BikeRepository.Find(b => b.Id == bikeId);
        return bike
            .AsNoTracking()
            .Select(b => new BikeRetrieveDto
            {
                BikeStationId = b.BikeStationId,
                BikeStationName = b.BikeStation != null ? b.BikeStation.Name : null,
                Id = b.Id,
                CreatedOn = b.CreatedOn,
                IsActive = b.IsActive,
                Description = b.Description,
                LicensePlate = b.LicensePlate,
                Status = b.Status,
                UpdatedOn = b.UpdatedOn,
                LastLongitude = b.BikeLocationTrackings.FirstOrDefault() != null ? 
                    b.BikeLocationTrackings.FirstOrDefault()!.Longitude : null,
                LastLatitude = b.BikeLocationTrackings.FirstOrDefault() != null ? 
                    b.BikeLocationTrackings.FirstOrDefault()!.Latitude : null,
                IsRenting = b.BikeLocationTrackings.Any(bt => bt.IsActive)
            }).FirstOrDefault();
    }

    public async Task<List<BikeRetrieveDto>> GetBikes(string managerEmail)
    {
        var bikesRetrieveDtos =  (await _unitOfWork.BikeRepository.Find(b => 
            b.BikeStation != null && 
            b.BikeStation.BikeStationManagers.Any(bs => bs.Manager.Email == managerEmail)
        )).Select(b => new BikeRetrieveDto
        {
            BikeStationId = b.BikeStationId,
            BikeStationName = b.BikeStation != null ? b.BikeStation.Name : null,
            Id = b.Id,
            CreatedOn = b.CreatedOn,
            IsActive = b.IsActive,
            Description = b.Description,
            LicensePlate = b.LicensePlate,
            Status = b.Status,
            UpdatedOn = b.UpdatedOn,
            LastLongitude = b.BikeLocationTrackings.FirstOrDefault() != null ? 
                b.BikeLocationTrackings.FirstOrDefault()!.Longitude : null,
            LastLatitude = b.BikeLocationTrackings.FirstOrDefault() != null ? 
                b.BikeLocationTrackings.FirstOrDefault()!.Latitude : null,
            IsRenting = b.BikeLocationTrackings.Any(bt => bt.IsActive)
        }).AsNoTracking().ToList();

        return bikesRetrieveDtos;
    }
}
