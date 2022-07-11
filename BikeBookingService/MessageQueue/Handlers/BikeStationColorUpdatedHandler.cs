using BikeBookingService.DAL;
using BikeBookingService.MessageQueue.Publisher;
using BikeRental.MessageQueue.Events;
using BikeRental.MessageQueue.Handlers;
using Newtonsoft.Json;

namespace BikeBookingService.MessageQueue.Handlers;

public class BikeStationColorUpdatedHandler : IMessageQueueHandler
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMessageQueuePublisher _messageQueuePublisher;

    public BikeStationColorUpdatedHandler(IUnitOfWork unitOfWork, IMessageQueuePublisher messageQueuePublisher)
    {
        _unitOfWork = unitOfWork;
        _messageQueuePublisher = messageQueuePublisher;
    }
    
    public async Task Handle(string message)
    {
        var payload = JsonConvert.DeserializeObject<BikeStationColorUpdated>(message);
        foreach (var bikeStationColor in payload.BikeStationColors)
        {
            var bikesUpdated = (await _unitOfWork.BikeRepository
                .Find(x => x.BikeStationId == bikeStationColor.BikeStationId)).ToList();
            
            bikesUpdated.ForEach(b => b.Color = bikeStationColor.Color);
        }

        await _unitOfWork.SaveChangesAsync();
        await _messageQueuePublisher.PublishBikeLocationChangeCommand(payload.ManagerEmails);
    }
}
