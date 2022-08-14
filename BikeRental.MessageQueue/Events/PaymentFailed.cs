namespace BikeRental.MessageQueue.Events;

public class PaymentFailed : BaseMessage
{
    public string Email { get; set; } = null!;
    public double Amount { get; set; }
    public string? FailureMessage { get; set; }
    public DateTime FailedOn { get; set; }
}
