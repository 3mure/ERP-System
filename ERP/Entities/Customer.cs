using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BuildingBlocks.SharedEntities;

namespace Catalog_Service.Entities;

/// <summary>
/// Full customer profile used by the CRM module (retail / POS).
/// </summary>
[Table("customers")]
public class Customer : BaseEntity
{
    [Required, MaxLength(150)]
    public string NameEn { get; set; } = null!;

    [MaxLength(150)]
    public string? NameAr { get; set; }

    [MaxLength(150), EmailAddress]
    public string? Email { get; set; }

    [Required, MaxLength(20)]
    public string Phone { get; set; } = null!;

    public string? PasswordHash { get; set; }

    [MaxLength(50)]
    public string Type { get; set; } = "Individual";

    [ForeignKey(nameof(CustomerGroup))]
    public int? CustomerGroupId { get; set; }

    public CustomerGroup? CustomerGroup { get; set; }

    [MaxLength(100)]
    public string? ReferralSource { get; set; }

    [ForeignKey(nameof(RegisteredByUser))]
    public int? RegisteredBy { get; set; }

    public User? RegisteredByUser { get; set; }

    [MaxLength(100)]
    public string? Area { get; set; }

    public string? FullAddress { get; set; }

    [MaxLength(100)]
    public string? Industry { get; set; }

    public int LoyaltyPoints { get; set; }

    public ICollection<SalesOrder> SalesOrders { get; set; } = new List<SalesOrder>();
}
