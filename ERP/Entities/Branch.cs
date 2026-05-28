using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BuildingBlocks.SharedEntities;

namespace Catalog_Service.Entities;

/// <summary>
/// A physical retail location or store.
/// </summary>
[Table("branches")]
public class Branch : BaseEntity
{
    [Required, MaxLength(100)]
    public string Name { get; set; } = null!;

    public string? Address { get; set; }

    public ICollection<Warehouse> Warehouses { get; set; } = new List<Warehouse>();
    public ICollection<SalesOrder> SalesOrders { get; set; } = new List<SalesOrder>();
    public ICollection<PosShift> PosShifts { get; set; } = new List<PosShift>();
}
