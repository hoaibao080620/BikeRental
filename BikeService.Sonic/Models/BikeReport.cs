using System.ComponentModel.DataAnnotations.Schema;
using Shared.Models;

namespace BikeService.Sonic.Models;

public class BikeReport : BaseEntity
{
    public int BikeId { get; set; }
    [ForeignKey(nameof(BikeId))]
    public Bike Bike { get; set; } = null!;

    public int AssignToId { get; set; }
    [ForeignKey(nameof(AssignToId))]
    public Manager AssignTo { get; set; } = null!;
    public DateTime? CompletedOn { get; set; }
    public int AccountId { get; set; }
    [ForeignKey(nameof(AccountId))]
    public Account Account { get; set; } = null!;

    public string ReportDescription { get; set; } = null!;
    public bool IsResolved { get; set; }
}
