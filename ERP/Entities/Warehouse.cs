using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BuildingBlocks.SharedEntities;

namespace Catalog_Service.Entities;

/// <summary>
/// A storage location linked to a branch.
/// </summary>
[Table("warehouses")]
public class Warehouse : BaseEntity
{
    [Required, MaxLength(100)]
    public string Name { get; set; } = null!;

    [ForeignKey(nameof(Branch))]
    public int? BranchId { get; set; }

    public Branch? Branch { get; set; }

    public ICollection<Inventory> Inventories { get; set; } = new List<Inventory>();
    public ICollection<StockTransfer> OutgoingTransfers { get; set; } = new List<StockTransfer>();
    public ICollection<StockTransfer> IncomingTransfers { get; set; } = new List<StockTransfer>();
}
