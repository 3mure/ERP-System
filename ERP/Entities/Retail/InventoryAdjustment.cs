using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BuildingBlocks.SharedEntities;

namespace Catalog_Service.Entities.Retail;

/// <summary>
/// Logs every manual stock correction made by a manager.
/// </summary>
[Table("inventory_adjustments", Schema = "retail")]
public class InventoryAdjustment : BaseEntity
{
    [ForeignKey(nameof(Product))]
    public int ProductId { get; set; }

    public Product Product { get; set; } = null!;

    [ForeignKey(nameof(Warehouse))]
    public int WarehouseId { get; set; }

    public Warehouse Warehouse { get; set; } = null!;

    [Required, MaxLength(20)]
    public string AdjustmentType { get; set; } = null!;

    public int Quantity { get; set; }

    [Required]
    public string Reason { get; set; } = null!;

    [ForeignKey(nameof(AdjustedByUser))]
    public int AdjustedBy { get; set; }

    public User AdjustedByUser { get; set; } = null!;
}
