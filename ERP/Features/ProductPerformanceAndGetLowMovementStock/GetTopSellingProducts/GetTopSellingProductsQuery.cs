using MediatR;

namespace Catalog_Service.Features.ProductPerformanceAndGetLowMovementStock.GetTopSellingProducts
{
    public record GetTopSellingProductsQuery() : IRequest<List<TopSellingProductDto>>;
}
