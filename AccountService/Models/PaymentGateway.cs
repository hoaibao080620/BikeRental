using System.ComponentModel.DataAnnotations;
using Shared.Models;

namespace AccountService.Models;

public class PaymentGateway : BaseEntity
{
    [Required]
    public string Name { get; set; } = null!;
}