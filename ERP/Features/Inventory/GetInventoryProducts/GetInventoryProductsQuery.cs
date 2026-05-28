using MediatR;

namespace Catalog_Service.Features.Inventory.GetInventoryProducts
{
    public record GetInventoryProductsQuery(
      string? Search,
      string WarehouseId,
      string Status,
      int Page
  ) : IRequest<GetInventoryProductsResult>;

}
