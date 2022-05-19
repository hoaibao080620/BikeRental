using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Shared.Models;

namespace AccountService.Models;

public class Account : BaseEntity
{
    public Guid AccountCode { get; set; }
    public string? CreditCard { get; set; }
    public double Balance { get; set; }
    [Required]
    public int UserId { get; set; }
    [ForeignKey("UserId")]
    public User User { get; set; } = null!;
}