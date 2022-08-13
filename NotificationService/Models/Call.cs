using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace NotificationService.Models;

public class Call
{
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = null!;
    public double Duration { get; set; }
    public string CallerCountry { get; set; } = null!;
    public string CalledCountry { get; set; } = null!;
    public string From { get; set; } = null!;
    public string To { get; set; } = null!;
    public string Status { get; set; } = null!;
    public string Direction { get; set; } = null!;
    public DateTime CalledOn { get; set; }
    public string? RecordingUrl { get; set; }
    public string CallSid { get; set; } = null!;
    public string? AnsweredBy { get; set; }
    public string? ManagerCaller { get; set; }
    public string? ManagerReceiver { get; set; }
}
