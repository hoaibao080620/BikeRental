namespace BikeRental.MessageQueue.Events;

public class PaymentSucceeded : BaseMessage
{
    public string Email { get; set; } = null!;
    public double Amount { get; set; }
    public DateTime PaymentOn { get; set; }
}
