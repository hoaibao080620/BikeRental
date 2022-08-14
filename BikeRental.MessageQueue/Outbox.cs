using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace BikeRental.MessageQueue;

public class Outbox
{
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = null!;
    public string Payload { get; set; } = null!;
    public string Topic { get; set; } = null!;
    public bool IsPublished { get; set; }
    public DateTime CreatedOn { get; set; }
}
