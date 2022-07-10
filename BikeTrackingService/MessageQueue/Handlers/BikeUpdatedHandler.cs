using BikeRental.MessageQueue.Events;
using BikeRental.MessageQueue.Handlers;
using BikeTrackingService.DAL;
using Newtonsoft.Json;

namespace BikeTrackingService.MessageQueue.Handlers;

public class BikeUpdatedHandler : IMessageQueueHandler
{
    private readonly IUnitOfWork _unitOfWork;

    public BikeUpdatedHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }
    
    public async Task Handle(string message)
    {
        var payload = JsonConvert.DeserializeObject<BikeUpdated>(message);
        var bike = await _unitOfWork.BikeRepository.GetById(payload.Id);
        if (bike is null) return;

        bike.BikeStationId = payload.BikeStationId ?? bike.BikeStationId;
        bike.BikeStationName = payload.BikeStationName ?? bike.BikeStationName;
        bike.UpdatedOn = DateTime.UtcNow;
        bike.Description = payload.Description ?? bike.Description;
        bike.LicensePlate = payload.LicensePlate ?? bike.LicensePlate;
        bike.Color = payload.Color ?? bike.Color;

        await _unitOfWork.SaveChangesAsync();
    }
}
