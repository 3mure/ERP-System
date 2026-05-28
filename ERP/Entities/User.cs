using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BuildingBlocks.SharedEntities;
using Catalog_Service.Entities.Retail;

namespace Catalog_Service.Entities;

/// <summary>
/// Represents every person who logs into the system — cashiers, sales staff, managers, and admins.
/// </summary>
[Table("users")]
public class User : BaseEntity
{
    [Required, MaxLength(150)]
    public string Name { get; set; } = null!;

    [MaxLength(150), EmailAddress]
    public string? Email { get; set; }

    [MaxLength(20)]
    public string? Phone { get; set; }

    /// <summary>Bcrypt-hashed password — never stored as plain text.</summary>
    [Required]
    public string PasswordHash { get; set; } = null!;

    [ForeignKey(nameof(Role))]
    public int RoleId { get; set; }

    public Role Role { get; set; } = null!;

    public ICollection<SalesOrder> SalesOrders { get; set; } = new List<SalesOrder>();
    public ICollection<InventoryAdjustment> Adjustments { get; set; } = new List<InventoryAdjustment>();
    public ICollection<StockTransfer> AuthorizedTransfers { get; set; } = new List<StockTransfer>();
    public ICollection<PosShift> PosShifts { get; set; } = new List<PosShift>();
}
