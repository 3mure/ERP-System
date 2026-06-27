using System.ComponentModel.DataAnnotations;

namespace Catalog_Service.Features.Inventory.TransferStock
{
    public class TransferStockRequest
    {
        [Required]
        public string TransferReference { get; set; }   // "TRF-2026-089"

        [Required]
        public int FromWarehouseId { get; set; }   

        [Required]
        public int ToWarehouseId { get; set; }   

        [Required, MinLength(1)]
        public List<TransferItemRequest> Items { get; set; }

        [Required]
        public int AuthorizedById { get; set; }  

        public string? Notes { get; set; }
    }

    public class TransferItemRequest
    {
        [Required]
        public int ProductId { get; set; }   // "PRD-101"

        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1.")]
        public int Quantity { get; set; }
    }
}