using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BuildingBlocks.SharedEntities;

namespace Catalog_Service.Entities.Retail;

/// <summary>
/// Immutable history of every change made to an invoice.
/// </summary>
[Table("invoice_audit_log", Schema = "retail")]
public class InvoiceAuditLog : BaseEntity
{
    [Required, MaxLength(50)]
    public string InvoiceNo { get; set; } = null!;

    [Required, MaxLength(50)]
    public string Action { get; set; } = null!;

    [ForeignKey(nameof(ChangedByUser))]
    public int? ChangedBy { get; set; }

    public User? ChangedByUser { get; set; }

    public string? ChangeSummary { get; set; }

    public DateTime ChangedAt { get; set; } = DateTime.UtcNow;
}
