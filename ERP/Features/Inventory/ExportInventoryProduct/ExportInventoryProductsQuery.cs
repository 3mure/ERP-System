using MediatR;

namespace Catalog_Service.Features.Inventory.ExportInventoryProduct
{
    public record ExportInventoryProductsQuery(
     string? Search,
     string WarehouseId,
     string Status
    ): IRequest<byte[]>;

}
