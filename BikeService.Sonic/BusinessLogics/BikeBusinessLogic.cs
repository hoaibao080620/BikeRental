using AutoMapper;
using BikeService.Sonic.Const;
using BikeService.Sonic.DAL;
using BikeService.Sonic.Dtos;
using BikeService.Sonic.Dtos.Bike;
using BikeService.Sonic.Dtos.BikeOperation;
using BikeService.Sonic.Exceptions;
using BikeService.Sonic.Models;
using BikeService.Sonic.Services.Interfaces;

namespace BikeService.Sonic.BusinessLogics;

public class BikeBusinessLogic : IBikeBusinessLogic
{
    private readonly IBikeLocationHub _bikeLocationHub;
    private readonly IBikeStationManagerRepository _bikeStationManagerRepository;
    private readonly IBikeRepository _bikeRepository;
    private readonly IMapper _mapper;

    public BikeBusinessLogic(
        IBikeLocationHub bikeLocationHub, 
        IBikeStationManagerRepository bikeStationManagerRepository,
        IBikeRepository bikeRepository,
        IMapper mapper)
    {
        _bikeLocationHub = bikeLocationHub;
        _bikeStationManagerRepository = bikeStationManagerRepository;
        _bikeRepository = bikeRepository;
        _mapper = mapper;
    }

    public async Task<BikeRetrieveDto> GetBike(int id)
    {
        var bike = await _bikeRepository.GetById(id);
        return _mapper.Map<BikeRetrieveDto>(bike);
    }

    public async Task<List<BikeRetrieveDto>> GetBikes()
    {
        var bikes = await _bikeRepository.All();
        return _mapper.Map<List<BikeRetrieveDto>>(bikes);
    }

    public async Task AddBike(BikeInsertDto bikeInsertDto)
    {
        await _bikeRepository.Add(_mapper.Map<Bike>(bikeInsertDto));
        await _bikeRepository.SaveChanges();
    }

    public async Task UpdateBike(BikeUpdateDto bikeInsertDto)
    {
        await _bikeRepository.Update(_mapper.Map<Bike>(bikeInsertDto));
        await _bikeRepository.SaveChanges();
    }

    public async Task DeleteBike(int id)
    {
        var bike = await _bikeRepository.GetById(id);
        if (bike is null) throw new InvalidOperationException();
        
        await _bikeRepository.Delete(bike);
        await _bikeRepository.SaveChanges();
    }

    public async Task BikeChecking(BikeCheckingDto bikeCheckingDto)
    {
        var managerEmails = await _bikeStationManagerRepository.GetManagerEmailsByBikeId(bikeCheckingDto.BikeId);
        var bike = await _bikeRepository.GetById(bikeCheckingDto.BikeId);
        _ = bike ?? throw new BikeNotFoundException(bikeCheckingDto.BikeId);
        
        foreach (var managerEmail in managerEmails)
        {
            await _bikeLocationHub.SendBikeLocationsData(managerEmail, new BikeLocationDto
            {
                BikeId = bike.Id,
                Longitude = bikeCheckingDto.Longitude,
                Latitude = bikeCheckingDto.Latitude,
                Plate = bike.LicensePlate,
                Operation = BikeLocationOperation.AddBikeToMap
            });
        }
    }

    public async Task BikeCheckout(BikeCheckoutDto bikeCheckingDto)
    {
        var emails = await _bikeStationManagerRepository.GetManagerEmailsByBikeId(bikeCheckingDto.BikeId);
        var bike = await _bikeRepository.GetById(bikeCheckingDto.BikeId);
        _ = bike ?? throw new BikeNotFoundException(bikeCheckingDto.BikeId);
        
        foreach (var email in emails)
        {
            await _bikeLocationHub.SendBikeLocationsData(email, new BikeLocationDto
            {
                BikeId = bike.Id,
                Operation = BikeLocationOperation.RemoveBikeFromMap
            });
        }
    }

    public async Task UpdateBikeLocation(BikeLocationDto bikeLocationDto)
    {
        var emails = await _bikeStationManagerRepository.GetManagerEmailsByBikeId(bikeLocationDto.BikeId);
        var bike = await _bikeRepository.GetById(bikeLocationDto.BikeId);
        _ = bike ?? throw new BikeNotFoundException(bikeLocationDto.BikeId);

        bikeLocationDto.Plate = bike.LicensePlate;
        bikeLocationDto.Operation = BikeLocationOperation.UpdateBikeFromMap;

        foreach (var email in emails)
        {
            await _bikeLocationHub.SendBikeLocationsData(email, bikeLocationDto);
        }
    }
}