namespace UserService.Dtos;

public class OktaUserInsertDto
{
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string Password { get; set; } = null!;
    public string? GroupId { get; set; }
    public string Email { get; set; } = null!;
    public string PhoneNumber { get; set; } = null!;
}
