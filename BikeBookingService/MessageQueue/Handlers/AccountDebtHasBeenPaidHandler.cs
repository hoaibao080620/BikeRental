using BikeBookingService.Const;
using BikeBookingService.DAL;
using BikeRental.MessageQueue.Events;
using BikeRental.MessageQueue.Handlers;
using Newtonsoft.Json;

namespace BikeBookingService.MessageQueue.Handlers;

public class AccountDebtHasBeenPaidHandler : IMessageQueueHandler
{
    private readonly IUnitOfWork _unitOfWork;

    public AccountDebtHasBeenPaidHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }
    
    public async Task Handle(string message)
    {
        var payload = JsonConvert.DeserializeObject<AccountDebtHasBeenPaid>(message);

        if (payload is null) return;

        var bikeRentalNotFullyPaid = (await _unitOfWork.BikeRentalTrackingRepository
            .Find(x => x.Account.Email == payload.AccountEmail)).First();

        bikeRentalNotFullyPaid.PaymentStatus = PaymentStatus.APPROVED;
        await _unitOfWork.SaveChangesAsync();
    }
}
