using System.ComponentModel.DataAnnotations.Schema;
using BuildingBlocks.SharedEntities;

namespace Catalog_Service.Entities;

/// <summary>
/// Records every product return linked to its original order.
/// </summary>
[Table("returns")]
public class Return : BaseEntity
{
    [ForeignKey(nameof(SalesOrder))]
    public int OrderId { get; set; }

    public SalesOrder SalesOrder { get; set; } = null!;

    [ForeignKey(nameof(Product))]
    public int ProductId { get; set; }

    public Product Product { get; set; } = null!;

    public int Quantity { get; set; } = 1;

    public string? Reason { get; set; }

    [ForeignKey(nameof(ProcessedByUser))]
    public int? ProcessedBy { get; set; }

    public User? ProcessedByUser { get; set; }

    public DateTime ReturnDate { get; set; } = DateTime.UtcNow;
}
