using System.ComponentModel.DataAnnotations.Schema;
using Shared.Models;

namespace AccountService.Models;

public class AccountPoint : BaseEntity
{
    public double Point { get; set; }
    public int AccountId { get; set; }
    [ForeignKey("AccountId")]
    public Account Account { get; set; } = null!;
}