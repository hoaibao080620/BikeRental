namespace BikeRental.MessageQueue.Events;

public class AccountDebtHasBeenPaid : BaseMessage
{
    public string AccountEmail { get; set; } = null!;
}
