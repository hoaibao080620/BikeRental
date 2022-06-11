using AutoMapper;
using BikeService.Sonic.Const;
using BikeService.Sonic.DAL;
using BikeService.Sonic.Dtos;
using BikeService.Sonic.Dtos.Bike;
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
    }

    public async Task UpdateBike(BikeUpdateDto bikeInsertDto)
    {
        await _bikeRepository.Update(_mapper.Map<Bike>(bikeInsertDto));
    }

    public async Task DeleteBike(int id)
    {
        var bike = await _bikeRepository.GetById(id);
        if (bike is null) throw new InvalidOperationException();
        
        await _bikeRepository.Delete(bike);
    }

    public async Task BikeChecking(BikeCheckingDto bikeCheckingDto)
    {
        var managerEmails = await _bikeStationManagerRepository.GetManagerEmailsByBikeId(bikeCheckingDto.BikeId);
        // TODO: Get Bike information from db instead of hardcode

        foreach (var managerEmail in managerEmails)
        {
            await _bikeLocationHub.SendBikeLocationsData(managerEmail, new BikeLocationDto
            {
                BikeId = bikeCheckingDto.BikeId,
                Longitude = bikeCheckingDto.Longitude,
                Latitude = bikeCheckingDto.Latitude,
                Plate = Guid.NewGuid().ToString(),
                Operation = BikeLocationOperation.AddBikeToMap
            });
        }
    }

    public async Task BikeCheckout(BikeCheckoutDto bikeCheckingDto)
    {
        var emails = await _bikeStationManagerRepository.GetManagerEmailsByBikeId(bikeCheckingDto.BikeId);

        foreach (var email in emails)
        {
            // TODO: Get Bike information from db instead of hardcode
            await _bikeLocationHub.SendBikeLocationsData(email, new BikeLocationDto
            {
                BikeId = bikeCheckingDto.BikeId,
                Operation = BikeLocationOperation.RemoveBikeFromMap
            });
        }
    }

    public async Task UpdateBikeLocation(BikeLocationDto bikeLocationDto)
    {
        var emails = await _bikeStationManagerRepository.GetManagerEmailsByBikeId(bikeLocationDto.BikeId);
        bikeLocationDto.Plate = Guid.NewGuid().ToString();
        bikeLocationDto.Operation = BikeLocationOperation.UpdateBikeFromMap;

        foreach (var email in emails)
        {
            await _bikeLocationHub.SendBikeLocationsData(email, bikeLocationDto);
        }
    }
}