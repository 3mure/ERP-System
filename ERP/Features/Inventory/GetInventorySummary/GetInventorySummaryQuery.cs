using Catalog_Service.Features.ProductsFeature.GetInventorySummary;
using MediatR;

namespace Catalog_Service.Features.Inventory.GetInventorySummary
{
    public record GetInventorySummaryQuery(string WarehouseId)
    : IRequest<InventorySummaryTableDto>;
}
