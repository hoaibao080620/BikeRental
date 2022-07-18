using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace AccountService.Models;

public class AccountTransaction
{
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = null!;
    public double Amount { get; set; }
    public DateTime TransactionTime { get; set; }
    public string AccountEmail { get; set; } = null!;
    public string AccountPhoneNumber { get; set; } = null!;
    public DateTime CreatedOn { get; set; }
}
