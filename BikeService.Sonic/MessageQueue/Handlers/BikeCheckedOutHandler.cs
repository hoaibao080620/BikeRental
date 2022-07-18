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
        if(bike is null) return;

        bike.Status = BikeStatus.Available;
        if (bike.BikeStationId != payload.BikeStationId)
        {
            var oldBikeStation = await _unitOfWork.BikeStationRepository.GetById(bike.BikeStationId!.Value);
            var newBikeStation = await _unitOfWork.BikeStationRepository.GetById(payload.BikeStationId);

            oldBikeStation!.UsedParkingSpace--;
            newBikeStation!.UsedParkingSpace++;
            
            bike.BikeStationId = payload.BikeStationId;
            var bikeStationColor = (await _unitOfWork.BikeStationColorRepository
                .Find(x => x.BikeStationId == payload.BikeStationId)).FirstOrDefault();
            await _messageQueuePublisher.PublishBikeUpdatedEvent(new BikeUpdated
            {
                Id = bike.Id,
                BikeStationId = newBikeStation.Id,
                BikeStationName = newBikeStation.Name,
                Color = bikeStationColor?.Color
            });
        }
        await _unitOfWork.SaveChangesAsync();
    }
}
