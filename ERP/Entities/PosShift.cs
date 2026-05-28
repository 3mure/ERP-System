using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BuildingBlocks.SharedEntities;

namespace Catalog_Service.Entities;

/// <summary>
/// Tracks a cashier's work session at a specific branch.
/// </summary>
[Table("pos_shifts")]
public class PosShift : BaseEntity
{
    [ForeignKey(nameof(Cashier))]
    public int CashierId { get; set; }

    public User Cashier { get; set; } = null!;

    [ForeignKey(nameof(Branch))]
    public int BranchId { get; set; }

    public Branch Branch { get; set; } = null!;

    public DateTime OpenedAt { get; set; } = DateTime.UtcNow;

    public DateTime? ClosedAt { get; set; }

    [Column(TypeName = "decimal(12,2)")]
    public decimal OpeningCash { get; set; }

    [Column(TypeName = "decimal(12,2)")]
    public decimal? ClosingCash { get; set; }

    [Column(TypeName = "decimal(12,2)")]
    public decimal TotalSales { get; set; }

    [MaxLength(20)]
    public string Status { get; set; } = "open";
}
