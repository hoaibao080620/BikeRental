namespace UserService.Dtos;

public class UserUpdateDto
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Address { get; set; } = null;
    public DateTime? DateOfBirth { get; set; } = null;
    public string? RoleName { get; set; }
    public string? Password { get; set; }
    public string? PhoneNumber { get; set; }
    public string? ImageBase64 { get; set; }
}
