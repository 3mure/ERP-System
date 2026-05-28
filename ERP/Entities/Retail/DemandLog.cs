using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BuildingBlocks.SharedEntities;

namespace Catalog_Service.Entities.Retail;

/// <summary>
/// Captures items customers wanted but couldn't buy due to stock-outs.
/// </summary>
[Table("demand_log", Schema = "retail")]
public class DemandLog : BaseEntity
{
    [ForeignKey(nameof(Product))]
    public int? ProductId { get; set; }

    public Product? Product { get; set; }

    [MaxLength(200)]
    public string? ProductName { get; set; }

    public int RequestsCount { get; set; } = 1;

    [MaxLength(50)]
    public string? Source { get; set; }

    [ForeignKey(nameof(Branch))]
    public int? BranchId { get; set; }

    public Branch? Branch { get; set; }

    [ForeignKey(nameof(LoggedByUser))]
    public int? LoggedBy { get; set; }

    public User? LoggedByUser { get; set; }

    public DateTime LastRequested { get; set; } = DateTime.UtcNow;
}
