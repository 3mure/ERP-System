using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BuildingBlocks.SharedEntities;

namespace Catalog_Service.Entities;

/// <summary>
/// Defines permission levels in the system.
/// </summary>
[Table("roles")]
public class Role : BaseEntity
{
    /// <summary>Admin | Manager | Cashier | Sales Associate</summary>
    [Required, MaxLength(50)]
    public string Name { get; set; } = null!;

    public ICollection<User> Users { get; set; } = new List<User>();
}
