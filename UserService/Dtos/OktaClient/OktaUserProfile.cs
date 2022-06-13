namespace UserService.Dtos.OktaClient;

public class OktaUserProfile
{
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string MobilePhone { get; set; } = null!;
}