namespace MessageQueue.Events;

public class UserDeleted : BaseMessage
{
    public int UserId { get; set; }
}