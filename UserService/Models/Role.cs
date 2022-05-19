using Shared.Models;

namespace UserService.Models;

public class Role : BaseEntity
{
    public string Name { get; set; } = null!;
}
