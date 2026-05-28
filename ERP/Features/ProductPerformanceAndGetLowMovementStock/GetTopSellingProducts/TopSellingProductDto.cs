namespace Catalog_Service.Features.ProductPerformanceAndGetLowMovementStock.GetTopSellingProducts
{
    public record TopSellingProductDto(
        string Name,
        int SoldUnits,
        string Percentage   // e.g. "18.5%" — this product's share of ALL units sold
    );
}