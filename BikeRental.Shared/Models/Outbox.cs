namespace Shared.Models;

public class Outbox
{
    public string Id { get; set; } = null!;
    public string Payload { get; set; } = null!;
    public string Topic { get; set; } = null!;
    public bool IsPublished { get; set; }
    public DateTime CreatedOn { get; set; }
}
