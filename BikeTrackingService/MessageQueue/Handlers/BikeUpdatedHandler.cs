using BikeRental.MessageQueue.Events;
using BikeRental.MessageQueue.Handlers;
using BikeService.Sonic.DAL;
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
        var bike = (await _unitOfWork.BikeRepository.Find(x => x.ExternalId == payload.Id)).First();

        bike.BikeStationId = payload.BikeStationId;
        bike.BikeStationName = payload.BikeStationName;
        bike.UpdatedOn = DateTime.UtcNow;
        bike.Description = payload.Description;
        bike.LicensePlate = payload.LicensePlate;

        await _unitOfWork.SaveChangesAsync();
    }
}
