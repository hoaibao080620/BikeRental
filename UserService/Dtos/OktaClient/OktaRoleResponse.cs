namespace UserService.Dtos.OktaClient;

public class OktaRoleResponse
{
    public string Id { get; set; } = null!;
    public DateTime Created { get; set; }
    public OktaRoleProfile Profile { get; set; } = null!;
}