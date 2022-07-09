using BikeRental.MessageQueue.Events;
using BikeRental.MessageQueue.Handlers;
using BikeService.Sonic.DAL;
using BikeTrackingService.DAL;
using Newtonsoft.Json;

namespace BikeTrackingService.MessageQueue.Handlers;

public class BikeDeletedHandler : IMessageQueueHandler
{
    private readonly IUnitOfWork _unitOfWork;

    public BikeDeletedHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }
    
    public async Task Handle(string message)
    {
        var payload = JsonConvert.DeserializeObject<BikeDeleted>(message);
        var bike = await _unitOfWork.BikeRepository.GetById(payload.Id);

        if (bike is null) return;
        await _unitOfWork.BikeRepository.Delete(bike);
        await _unitOfWork.SaveChangesAsync();
    }
}
