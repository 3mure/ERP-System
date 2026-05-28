using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BuildingBlocks.SharedEntities;
using Catalog_Service.Entities.Retail;

namespace Catalog_Service.Entities;

/// <summary>
/// The master sales transaction record — one row per invoice.
/// </summary>
[Table("sales_orders")]
public class SalesOrder : BaseEntity
{
    [Required, MaxLength(50)]
    public string OrderCode { get; set; } = null!;

    [Required, MaxLength(50)]
    public string InvoiceNo { get; set; } = null!;

    [ForeignKey(nameof(Customer))]
    public int? CustomerId { get; set; }

    public Customer? Customer { get; set; }

    [ForeignKey(nameof(Cashier))]
    public int CashierId { get; set; }

    public User Cashier { get; set; } = null!;

    [ForeignKey(nameof(Branch))]
    public int? BranchId { get; set; }

    public Branch? Branch { get; set; }

    [ForeignKey(nameof(PaymentMethod))]
    public int? PaymentMethodId { get; set; }

    public PaymentMethod? PaymentMethod { get; set; }

    [Column(TypeName = "decimal(12,2)")]
    public decimal TotalAmount { get; set; }

    [Required, MaxLength(20)]
    public string Status { get; set; } = "Completed";

    public DateTime OrderDate { get; set; } = DateTime.UtcNow;

    public ICollection<SalesOrderItem> Items { get; set; } = new List<SalesOrderItem>();
    public ICollection<Return> Returns { get; set; } = new List<Return>();
}
