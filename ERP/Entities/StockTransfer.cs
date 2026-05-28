using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BuildingBlocks.SharedEntities;

namespace Catalog_Service.Entities;

/// <summary>
/// Records the movement of stock between two warehouses.
/// </summary>
[Table("stock_transfers")]
public class StockTransfer : BaseEntity
{
    [Required, MaxLength(50)]
    public string TransferReference { get; set; } = null!;

    [ForeignKey(nameof(FromWarehouse))]
    public int FromWarehouseId { get; set; }

    public Warehouse FromWarehouse { get; set; } = null!;

    [ForeignKey(nameof(ToWarehouse))]
    public int ToWarehouseId { get; set; }

    public Warehouse ToWarehouse { get; set; } = null!;

    [ForeignKey(nameof(AuthorizedByUser))]
    public int AuthorizedBy { get; set; }

    public User AuthorizedByUser { get; set; } = null!;

    public string? Notes { get; set; }

    [MaxLength(20)]
    public string Status { get; set; } = "completed";

    public ICollection<StockTransferItem> Items { get; set; } = new List<StockTransferItem>();
}
