using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BuildingBlocks.SharedEntities;

namespace Catalog_Service.Entities.Retail;

/// <summary>
/// Lookup table for how a sale was paid.
/// </summary>
[Table("payment_methods", Schema = "retail")]
public class PaymentMethod : BaseEntity
{
    [Required, MaxLength(50)]
    public string Name { get; set; } = null!;

    public ICollection<SalesOrder> SalesOrders { get; set; } = new List<SalesOrder>();
}
