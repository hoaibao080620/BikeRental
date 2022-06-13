namespace UserService.Dtos.OktaClient;

public class OktaUserResponse
{
    public string Id { get; set; } = null!;
    public string Status { get; set; } = null!;
    public DateTime Created { get; set; }
    public OktaUserProfile Profile { get; set; } = null!;
}