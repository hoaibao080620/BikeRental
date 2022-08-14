namespace BikeRental.MessageQueue.Events;

public class UserRoleUpdated : BaseMessage
{
    public string UserId { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string? PhoneNumber { get; set; }
    public string OriginalRole { get; set; } = null!;
    public string NewRole { get; set; } = null!;
}
