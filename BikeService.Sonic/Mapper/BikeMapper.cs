using BikeService.Sonic.Dtos.Bike;
using BikeService.Sonic.Models;

namespace BikeService.Sonic.Mapper;

public class BikeMapper
{
    public static BikeRetrieveDto Map(Bike b)
    {
        return new BikeRetrieveDto
        {
            BikeStationId = b.BikeStationId,
            BikeStationName = b.BikeStation?.Name,
            Id = b.Id,
            CreatedOn = b.CreatedOn,
            IsActive = b.IsActive,
            Description = b.Description,
            LicensePlate = b.LicensePlate,
            Status = b.Status,
            UpdatedOn = b.UpdatedOn,
            LastLongitude = b.BikeLocationTrackings.FirstOrDefault()!.Longitude,
            LastLatitude = b.BikeLocationTrackings.FirstOrDefault()!.Latitude,
            IsRenting = b.BikeLocationTrackings.Any(bt => bt.IsActive)
        };
    }
}
