namespace UserService.Dtos;

public class OktaUserInsertParam
{
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string? Address { get; set; }
    public string Email { get; set; } = null!;
    public string? PhoneNumber { get; set; } = null!;
    public DateTime? DateOfBirth { get; set; }
    public string? RoleName { get; set; }
    public string Password { get; set; } = null!;
}
