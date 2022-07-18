using BikeRental.MessageQueue.Events;
using BikeRental.MessageQueue.Handlers;
using BikeService.Sonic.Const;
using BikeService.Sonic.DAL;
using Newtonsoft.Json;

namespace BikeService.Sonic.MessageQueue.Handlers;

public class BikeCheckedInEventHandler : IMessageQueueHandler
{
    private readonly IUnitOfWork _unitOfWork;

    public BikeCheckedInEventHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }
    
    public async Task Handle(string message)
    {
        var payload = JsonConvert.DeserializeObject<BikeCheckedIn>(message);
        if (payload is null) return;

        var bike = await _unitOfWork.BikeRepository.GetById(payload.BikeId);
        if(bike is null) return;

        bike.Status = BikeStatus.InUsed;
        await _unitOfWork.SaveChangesAsync();
    }
}
