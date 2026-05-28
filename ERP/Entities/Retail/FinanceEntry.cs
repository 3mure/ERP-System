using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BuildingBlocks.SharedEntities;

namespace Catalog_Service.Entities.Retail;

/// <summary>
/// General ledger for all money moving in and out of the business.
/// </summary>
[Table("finance_entries", Schema = "retail")]
public class FinanceEntry : BaseEntity
{
    [Required, MaxLength(20)]
    public string EntryType { get; set; } = null!;

    [MaxLength(100)]
    public string? Category { get; set; }

    [Column(TypeName = "decimal(12,2)")]
    public decimal Amount { get; set; }

    public int? ReferenceId { get; set; }

    [MaxLength(50)]
    public string? ReferenceType { get; set; }

    [ForeignKey(nameof(Branch))]
    public int? BranchId { get; set; }

    public Branch? Branch { get; set; }

    [ForeignKey(nameof(RecordedByUser))]
    public int? RecordedBy { get; set; }

    public User? RecordedByUser { get; set; }

    public DateTime EntryDate { get; set; } = DateTime.UtcNow;

    public string? Notes { get; set; }
}
