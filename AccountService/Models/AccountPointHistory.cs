using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace AccountService.Models;

public class AccountPointHistory
{
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = null!;
    public double Point { get; set; }
    public string AccountEmail { get; set; } = null!;
    public DateTime CreatedOn { get; set; }
}
