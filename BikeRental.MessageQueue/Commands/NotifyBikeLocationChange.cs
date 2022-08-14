namespace BikeRental.MessageQueue.Commands;

public class NotifyBikeLocationChange : BaseMessage
{
    public List<string> ManagerEmails { get; set; } = null!;
}
