using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Shared.Models;

namespace BikeService.Sonic.Models;

public class Bike : BaseEntity
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public new string Id { get; set; } = null!;

    public string LicensePlate { get; set; } = null!;
    public string? Description { get; set; }
    public string BikeStationId { get; set; } = null!;
}