using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BuildingBlocks.SharedEntities;

namespace Catalog_Service.Entities;

/// <summary>
/// Segments customers for targeted reporting and pricing rules.
/// </summary>
[Table("customer_groups")]
public class CustomerGroup : BaseEntity
{
    [Required, MaxLength(100)]
    public string Name { get; set; } = null!;

    public ICollection<Customer> Customers { get; set; } = new List<Customer>();
}
