namespace UserService.Dtos;

public class ForgetPasswordDto
{
    public string PhoneNumber { get; set; } = null!;
    public string NewPassword { get; set; } = null!;
}
