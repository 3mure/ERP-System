namespace Catalog_Service.Features.Inventory.TransferStock
{
    public record TransferStockResult(
     bool Success,
     string Message,
     string? TransferReference = null,
     ManifestDto? Manifest = null   // printable data for the driver
    );

    public record ManifestDto(
     string TransferReference,
     string FromWarehouse,
     string ToWarehouse,
     string AuthorizedBy,
     string TransferDate,
     string? Notes,
     List<ManifestItem> Items
    );

    public record ManifestItem(
     string ProductCode,
     string ProductName,
     int Quantity
    );
}