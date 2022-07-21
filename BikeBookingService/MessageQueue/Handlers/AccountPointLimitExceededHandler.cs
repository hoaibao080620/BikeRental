using BikeBookingService.Const;
using BikeBookingService.DAL;
using BikeRental.MessageQueue.Events;
using BikeRental.MessageQueue.Handlers;
using Newtonsoft.Json;

namespace BikeBookingService.MessageQueue.Handlers;

public class AccountPointLimitExceededHandler : IMessageQueueHandler
{
    private readonly IUnitOfWork _unitOfWork;

    public AccountPointLimitExceededHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }
    
    public async Task Handle(string message)
    {
        var payload = JsonConvert.DeserializeObject<AccountPointLimitExceeded>(message);
        if (payload is null) return;

        var bikeBooking = (await _unitOfWork.BikeRentalTrackingRepository
                .Find(x => x.Account.Email == payload.AccountEmail && x.PaymentStatus == PaymentStatus.PENDING))
            .FirstOrDefault();

        if (bikeBooking is null) return;

        bikeBooking.PaymentStatus = PaymentStatus.NOT_FULLY_PAID;
        await _unitOfWork.SaveChangesAsync();
    }
}
