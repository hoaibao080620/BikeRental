namespace BikeRental.MessageQueue.Events;

public class AccountPointLimitExceeded : BaseMessage
{
    public string AccountId { get; set; } = null!;
    public string AccountEmail { get; set; } = null!;
}
