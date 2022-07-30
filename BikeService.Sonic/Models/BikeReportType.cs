using System.ComponentModel.DataAnnotations;
using Shared.Models;

namespace BikeService.Sonic.Models;

public class BikeReportType : BaseEntity
{
    [MaxLength(50)]
    public string Name { get; set; } = null!;
}
