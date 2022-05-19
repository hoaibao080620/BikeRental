using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Shared.Models;

namespace AccountService.Models;

public class AccountTransaction : BaseEntity
{
    public double Amount { get; set; }
    public DateTime TransactionTime { get; set; }
    [Required]
    public int AccountId { get; set; }
    [Required]
    public int TransactionTypeId { get; set; }
    [Required]
    public int PaymentGatewayId { get; set; }
    [ForeignKey("AccountId")]
    public Account Account { get; set; } = null!;

    [ForeignKey("TransactionTypeId")]
    public TransactionType TransactionType { get; set; } = null!;

    [ForeignKey("PaymentGatewayId")]
    public PaymentGateway PaymentGateway { get; set; } = null!;
}