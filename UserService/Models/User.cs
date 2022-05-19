using System.ComponentModel.DataAnnotations.Schema;
using Shared.Models;

namespace UserService.Models;

public class User : BaseEntity
{
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string? Address { get; set; }
    public string Email { get; set; } = null!;
    public string? PhoneNumber { get; set; }
    public string? OktaUserId { get; set; }
    public int RoleId { get; set; }
    [ForeignKey("RoleId")]
    public Role Role { get; set; } = null!;
}
