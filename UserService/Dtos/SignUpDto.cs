using System.ComponentModel.DataAnnotations;

namespace UserService.Dtos;

public class SignUpDto
{
    [Required]
    public string FirstName { get; set; } = null!;
    [Required]
    public string LastName { get; set; } = null!;
    public string? Address { get; set; }
    [Required]
    public string PhoneNumber { get; set; } = null!;
    public DateTime? DateOfBirth { get; set; }
    [Required]
    public string Password { get; set; } = null!;
}
