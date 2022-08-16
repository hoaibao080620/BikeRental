namespace AccountService.Dto;

public class AccountProfileDto
{
    public string Id { get; set; } = null!;
    public double Point { get; set; }
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string PhoneNumber { get; set; } = null!;
    public bool IsActive { get; set; }
    public string? Image { get; set; }
}
