using System.ComponentModel.DataAnnotations.Schema;
using BuildingBlocks.SharedEntities;

namespace Catalog_Service.Entities;

/// <summary>
/// Each product line inside a sales order.
/// </summary>
[Table("sales_order_items")]
public class SalesOrderItem : BaseEntity
{
    [ForeignKey(nameof(SalesOrder))]
    public int OrderId { get; set; }

    public SalesOrder SalesOrder { get; set; } = null!;

    [ForeignKey(nameof(Product))]
    public int ProductId { get; set; }

    public Product Product { get; set; } = null!;

    [ForeignKey(nameof(Warehouse))]
    public int? WarehouseId { get; set; }

    public Warehouse? Warehouse { get; set; }

    public int Quantity { get; set; }

    [Column(TypeName = "decimal(12,2)")]
    public decimal UnitPrice { get; set; }
    public decimal? CostPrice { get; set; }

    [NotMapped]
    public decimal Subtotal => Quantity * UnitPrice;
}
