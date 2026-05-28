using BuildingBlocks.FullEntities.Catalog_Service_Entities;
using BuildingBlocks.FullEntities.Catalog_Service_Entities.Occasions;
using BuildingBlocks.SharedEntities;
using Catalog_Service.Entities.Retail;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Catalog_Service.Entities
{
    /// <summary>
    /// Single product entity for e-commerce catalog and retail/POS (shared table).</summary>
    public class Product : BaseEntity
    {
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
        public int MinStock { get; set; }
        public int MaxStock { get; set; }
        public bool IsAvailable { get; set; }

        public decimal? DiscountedPrice { get; set; }
        public int? ActiveOfferId { get; set; }

        public decimal AverageRating { get; set; } = 0;
        public int TotalReviews { get; set; } = 0;

        [ForeignKey(nameof(Brand))]
        public int? BrandId { get; set; }
        public int CategoryId { get; set; }

        public virtual Brand? Brand { get; set; }
        public virtual Category Category { get; set; } = null!;
        public virtual ICollection<ProductImage> Images { get; set; } = new List<ProductImage>();
        public virtual ICollection<ProductSpecification> Specifications { get; set; } = new List<ProductSpecification>();
        public virtual ICollection<ProductOccasion> ProductOccasions { get; set; } = new List<ProductOccasion>();
        public virtual ICollection<ProductReview> Reviews { get; set; } = new List<ProductReview>();
        public virtual ICollection<PriceHistory> PriceHistories { get; set; } = new List<PriceHistory>();

        // --- Retail / POS (same row; optional fields) ---
        /// <summary>POS SKU / internal code; unique when set.</summary>
        [MaxLength(50)]
        public string? ProductCode { get; set; }

        [Column(TypeName = "decimal(12,2)")]
        public decimal? CostPrice { get; set; }

        /// <summary>Optional retail category (separate from e-commerce <see cref="Category"/>).</summary>
        
        

        [ForeignKey(nameof(Supplier))]
        public int? SupplierId { get; set; }

        public virtual Supplier? Supplier { get; set; }

        /// <summary>Warehouse stock rows (retail).</summary>
        public virtual ICollection<Inventory> Inventories { get; set; } = new List<Inventory>();

        public virtual ICollection<SalesOrderItem> SalesOrderItems { get; set; } = new List<SalesOrderItem>();
        public virtual ICollection<Return> Returns { get; set; } = new List<Return>();
        public virtual ICollection<DemandLog> DemandLogs { get; set; } = new List<DemandLog>();
    }
}
