using System.ComponentModel.DataAnnotations;
using Shared.Models;

namespace UserService.Models;

public class Role : BaseEntity
{
    [MaxLength(100)]
    public string Name { get; set; } = null!;
    public string OktaRoleId { get; set; } = null!;
    [MaxLength(1000)]
    public string? Description { get; set; }
    public virtual ICollection<User> Users { get; set; } = null!;
}
