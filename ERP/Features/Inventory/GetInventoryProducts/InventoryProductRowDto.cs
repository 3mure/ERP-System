namespace Catalog_Service.Features.Inventory.GetInventoryProducts
{
    // <summary>
    /// One row in the inventory products table — all 9 columns.
    /// </summary>
    public record InventoryProductRowDto(
        string ProductId,         // "PRD-101"
        string ProductName,
        string Category,
        string BranchWarehouse,   // "Main Warehouse"
        int CurrentStock,
        int ReservedStock,
        int AvailableStock,    // CurrentStock - ReservedStock
        string Status,            // "In Stock" | "Low Stock" | "Out of Stock"
        string StatusColor,       // "green" | "yellow" | "red" — for badge
        string LastUpdate         // "2026-04-06"
    );
}