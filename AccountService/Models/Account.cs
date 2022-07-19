using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace AccountService.Models;

public class Account
{
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = null!;
    public double Point { get; set; }
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string PhoneNumber { get; set; } = null!;
    public string ExternalUserId { get; set; } = null!;
    public DateTime CreatedOn { get; set; } = DateTime.Now;
    public DateTime? UpdatedOn { get; set; } = DateTime.Now;
    public bool IsActive { get; set; }
}
