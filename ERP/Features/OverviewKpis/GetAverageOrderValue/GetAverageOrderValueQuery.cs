using Catalog_Service.Features.OverviewKpis.GetNetProfit;
using MediatR;

namespace Catalog_Service.Features.OverviewKpis.GetAverageOrderValue
{
    public record GetAverageOrderValueQuery(string DateRange) : IRequest<KpiResultDto>;
}
