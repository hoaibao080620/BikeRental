using BikeRental.MessageQueue.Events;
using BikeRental.MessageQueue.Handlers;
using BikeService.Sonic.Const;
using BikeService.Sonic.DAL;
using BikeService.Sonic.MessageQueue.Publisher;
using Newtonsoft.Json;

namespace BikeService.Sonic.MessageQueue.Handlers;

public class BikeCheckedOutHandler : IMessageQueueHandler
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMessageQueuePublisher _messageQueuePublisher;

    public BikeCheckedOutHandler(IUnitOfWork unitOfWork, IMessageQueuePublisher messageQueuePublisher)
    {
        _unitOfWork = unitOfWork;
        _messageQueuePublisher = messageQueuePublisher;
    }
    
    public async Task Handle(string message)
    {
        var payload = JsonConvert.DeserializeObject<BikeCheckedOut>(message);
        if (payload is null) return;

        var bike = await _unitOfWork.BikeRepository.GetById(payload.BikeId);
        var checkOutBikeStation = (await _unitOfWork.BikeStationRepository
            .Find(x => x.Bikes.Any(xx => xx.BikeCode == payload.BikeCode))).FirstOrDefault();
        if(bike is null || checkOutBikeStation is null) return;

        bike.Status = BikeStatus.Available;
        bike.UpdatedOn = DateTime.UtcNow;
        if (bike.BikeStationId != checkOutBikeStation.Id)
        {
            bike.BikeStationId = checkOutBikeStation.Id;
            var bikeStationColor = (await _unitOfWork.BikeStationColorRepository
                .Find(x => x.BikeStationId == checkOutBikeStation.Id)).FirstOrDefault();
            await _messageQueuePublisher.PublishBikeUpdatedEvent(new BikeUpdated
            {
                Id = bike.Id,
                BikeStationId = checkOutBikeStation.Id,
                BikeStationName = checkOutBikeStation.Name,
                Color = bikeStationColor?.Color,
                BikeStationCode = checkOutBikeStation.Code
            });
        }
        await _unitOfWork.SaveChangesAsync();
    }
}
