using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Shared.Models;

namespace AccountService.Models;

public class AccountTransaction
{
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = null!;
    public double Amount { get; set; }
    public DateTime TransactionTime { get; set; }
    [Required]
    public int AccountId { get; set; }
    public string TransactionType { get; set; } = null!;
    public DateTime CreatedOn { get; set; }
}
