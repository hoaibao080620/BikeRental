﻿using BikeService.Sonic.DAL;
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
                IsRenting = b.BikeLocationTrackings.Any(bt => bt.IsActive),
                LastAddress = b.BikeLocationTrackings.FirstOrDefault() != null ? 
                    b.BikeLocationTrackings.FirstOrDefault()!.Address : null,
                BikeStationColor = b.BikeStation!.BikeStationColors.FirstOrDefault() != null ? 
                    b.BikeStation!.BikeStationColors.FirstOrDefault()!.Color : null
            }).FirstOrDefaultAsync() ?? throw new BikeNotFoundException(bikeId);
    }

    public async Task<List<int>> GetBikeIdsOfManager(string managerEmail)
    {
        var bikeIds = (await _unitOfWork.BikeStationManagerRepository
                .Find(x => x.Manager.Email == managerEmail))
            .SelectMany(x => x.BikeStation.Bikes)
            .AsNoTracking()
            .Select(b => b.Id);

        return bikeIds.ToList();
    }
}
