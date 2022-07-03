using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace UserService.Models;

public class Role
{
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = null!;

    public string Name { get; set; } = null!;
    public string OktaRoleId { get; set; } = null!;
    public string? Description { get; set; }
    public DateTime CreatedOn { get; set; }
    public DateTime? UpdatedOn { get; set; }
}
