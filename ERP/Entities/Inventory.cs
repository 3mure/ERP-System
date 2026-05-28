using System.ComponentModel.DataAnnotations.Schema;
using BuildingBlocks.SharedEntities;

namespace Catalog_Service.Entities;

/// <summary>
/// Tracks the exact stock count for one product in one warehouse.
/// </summary>
[Table("inventory")]
public class Inventory : BaseEntity
{
    [ForeignKey(nameof(Product))]
    public int ProductId { get; set; }

    public Product Product { get; set; } = null!;

    [ForeignKey(nameof(Warehouse))]
    public int WarehouseId { get; set; }

    public Warehouse Warehouse { get; set; } = null!;

    public int CurrentStock { get; set; }

    public int ReservedStock { get; set; }

    public int DamagedStock { get; set; }

    [NotMapped]
    public int AvailableStock => CurrentStock - ReservedStock;

    [NotMapped]
    public string Status =>
        CurrentStock == 0 ? "Out of Stock" :
        Product != null && CurrentStock <= (Product.MinStock > 0 ? Product.MinStock : 10) ? "Low Stock" :
        "In Stock";

    public DateTime LastUpdate { get; set; } = DateTime.UtcNow;
}
