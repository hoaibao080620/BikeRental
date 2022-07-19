using BikeBookingService.DAL;
using BikeBookingService.Models;
using BikeRental.MessageQueue.Events;
using BikeRental.MessageQueue.Handlers;
using BikeService.Sonic.DAL;
using Newtonsoft.Json;

namespace BikeBookingService.MessageQueue.Handlers;

public class BikeCreatedHandler : IMessageQueueHandler
{
    private readonly IUnitOfWork _unitOfWork;

    public BikeCreatedHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }
    public async Task Handle(string message)
    {
        var payload = JsonConvert.DeserializeObject<BikeCreated>(message);

        await _unitOfWork.BikeRepository.Add(new Bike
        {
            Id = payload.Id,
            BikeStationId = payload.BikeStationId,
            BikeStationName = payload.BikeStationName,
            CreatedOn = DateTime.Now,
            Description = payload.Description,
            IsActive = true,
            LicensePlate = payload.LicensePlate,
            Color = payload.Color
        });

        await _unitOfWork.SaveChangesAsync();
        Console.WriteLine("Hello");
    }
}
