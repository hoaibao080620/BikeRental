using System.ComponentModel.DataAnnotations;

namespace Shared.Models;

public class BaseEntity
{
    [Key]
    public int Id { get; set; }
    public DateTime CreatedOn { get; set; }
    public DateTime? UpdatedOn { get; set; }
    public bool IsActive { get; set; }
}