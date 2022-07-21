using BikeBookingService.Const;
using BikeBookingService.DAL;
using BikeRental.MessageQueue.Events;
using BikeRental.MessageQueue.Handlers;
using Newtonsoft.Json;

namespace BikeBookingService.MessageQueue.Handlers;

public class AccountPointSubtractedHandler : IMessageQueueHandler
{
    private readonly IUnitOfWork _unitOfWork;

    public AccountPointSubtractedHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }
    public async Task Handle(string message)
    {
        var payload = JsonConvert.DeserializeObject<AccountPointSubtracted>(message);
        if (payload is null) return;

        var bikeBooking = (await _unitOfWork.BikeRentalTrackingRepository
                .Find(x => x.Account.Email == payload.AccountEmail && x.PaymentStatus == PaymentStatus.PENDING))
            .FirstOrDefault();

        if (bikeBooking is null) return;

        bikeBooking.PaymentStatus = PaymentStatus.APPROVED;
        await _unitOfWork.SaveChangesAsync();
    }
}
