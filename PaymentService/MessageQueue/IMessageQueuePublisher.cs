using BikeRental.MessageQueue.Events;

namespace PaymentService.MessageQueue;

public interface IMessageQueuePublisher
{
    Task PublishPaymentSucceededEvent(PaymentSucceeded paymentSucceeded);
    Task PublishPaymentFailedEvent(PaymentFailed paymentFailed);
}
