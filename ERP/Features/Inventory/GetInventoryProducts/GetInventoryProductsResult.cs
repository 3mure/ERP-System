using Catalog_Service.Features.Customers.GetCustomers;

namespace Catalog_Service.Features.Inventory.GetInventoryProducts
{
    public record GetInventoryProductsResult(
     List<InventoryProductRowDto> Data,
     PaginationDto Pagination
    );
}