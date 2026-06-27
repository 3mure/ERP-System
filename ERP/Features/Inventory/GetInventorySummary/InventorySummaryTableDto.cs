namespace Catalog_Service.Features.Inventory.GetInventorySummary
{
    public record InventorySummaryTableDto(
    string TotalStockQuantity,  // "14,500"
    string TotalStockValue,     // "EGP 250,000"
    int LowStockItems,       // count of products at or below threshold
    int OutOfStock,          // count of products with current_stock = 0
    int DamagedExpired       // sum of damaged_stock across all inventory rows
);
}