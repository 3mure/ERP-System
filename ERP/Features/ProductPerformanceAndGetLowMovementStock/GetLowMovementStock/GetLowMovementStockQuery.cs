using MediatR;

namespace Catalog_Service.Features.ProductPerformanceAndGetLowMovementStock.GetLowMovementStock
{
    public record GetLowMovementStockQuery() : IRequest<List<LowMovementStockDto>>;
}
