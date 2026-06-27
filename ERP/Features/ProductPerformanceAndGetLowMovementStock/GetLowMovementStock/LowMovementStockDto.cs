namespace Catalog_Service.Features.ProductPerformanceAndGetLowMovementStock.GetLowMovementStock
{
    public record LowMovementStockDto(
    string Name,
    int DaysWithoutSale,  // days since last completed sale for this product
    int TotalStock        // current total stock across all warehouses
    );
}