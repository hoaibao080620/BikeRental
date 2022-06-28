namespace UserService.Dtos.User;

public class UserProfileDto
{
    public string Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public DateTime DateOfBirth { get; set; }
    public string? Address { get; set; }
    public string Email { get; set; }
    public string Role { get; set; }
}
