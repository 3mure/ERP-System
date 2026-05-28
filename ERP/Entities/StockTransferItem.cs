using System.ComponentModel.DataAnnotations.Schema;
using BuildingBlocks.SharedEntities;

namespace Catalog_Service.Entities;

/// <summary>
/// Each line in a stock transfer.
/// </summary>
[Table("stock_transfer_items")]
public class StockTransferItem : BaseEntity
{
    [ForeignKey(nameof(StockTransfer))]
    public int TransferId { get; set; }

    public StockTransfer StockTransfer { get; set; } = null!;

    [ForeignKey(nameof(Product))]
    public int ProductId { get; set; }

    public Product Product { get; set; } = null!;

    public int Quantity { get; set; }
}
