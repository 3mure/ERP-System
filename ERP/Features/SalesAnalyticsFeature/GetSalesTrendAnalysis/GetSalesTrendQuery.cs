using MediatR;

namespace Catalog_Service.Features.SalesAnalyticsFeature.GetSalesTrendAnalysis
{
    public record GetSalesTrendQuery(string TrendFilter) : IRequest<List<SalesTrendPointDto>>;
}
