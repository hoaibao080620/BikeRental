using System.ComponentModel.DataAnnotations;

namespace UserService.Dtos;

public class UserInsertDto
{
    [Required]
    public string FirstName { get; set; } = null!;

    [Required]
    public string LastName { get; set; } = null!;

    [Required]
    public string Password { get; set; } = null!;

    public string? Address { get; set; }
    [Required]
    public string Email { get; set; } = null!;
    public string? PhoneNumber { get; set; } = null!;
    public DateTime? DateOfBirth { get; set; }
    public string? RoleName { get; set; }
}
