using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace NotificationService.Models;

public class Notification
{
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = null!;
    public string NotificationEmail { get; set; } = null!;
    public string NotificationContent { get; set; } = null!;
    public string NotificationType { get; set; } = null!;
    public bool IsOpen { get; set; }
    public bool IsSeen { get; set; }
    public bool IsHidden { get; set; }
    public DateTime CreatedOn { get; set; }
}
