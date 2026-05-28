using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BuildingBlocks.SharedEntities;

namespace Catalog_Service.Entities;

/// <summary>
/// A vendor who supplies products to the business.
/// </summary>
[Table("suppliers")]
public class Supplier : BaseEntity
{
    [Required, MaxLength(150)]
    public string Name { get; set; } = null!;

    [MaxLength(100)]
    public string? Type { get; set; }

    [MaxLength(150), EmailAddress]
    public string? ContactEmail { get; set; }

    [MaxLength(20)]
    public string? ContactPhone { get; set; }

    public ICollection<Product> Products { get; set; } = new List<Product>();
}
