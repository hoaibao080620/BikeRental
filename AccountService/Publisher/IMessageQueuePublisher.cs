using BikeRental.MessageQueue.Events;

namespace AccountService.Publisher;

public interface IMessageQueuePublisher
{
    Task PublishAccountPointSubtractedEvent(AccountPointSubtracted accountPointSubtracted);
    Task PublishAccountPointLimitExceededEvent(AccountPointLimitExceeded accountPointLimitExceeded);
    Task PublishAccountDebtHasBeenPaidEvent(AccountDebtHasBeenPaid accountDebtHasBeenPaid);
}
