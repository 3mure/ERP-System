using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BuildingBlocks.SharedEntities;

namespace Catalog_Service.Entities.Retail;

/// <summary>
/// Configurable thresholds that trigger automatic system alerts.
/// </summary>
[Table("alert_thresholds", Schema = "retail")]
public class AlertThreshold : BaseEntity
{
    [Required, MaxLength(100)]
    public string AlertType { get; set; } = null!;

    [Column(TypeName = "decimal(10,4)")]
    public decimal ThresholdValue { get; set; }

    public string? Description { get; set; }
}
