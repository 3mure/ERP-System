using Catalog_Service.Features.OverviewKpis.GetNetProfit;
using MediatR;

namespace Catalog_Service.Features.OverviewKpis.GetStockTurnoverRate
{
    public record GetStockTurnoverRateQuery(string DateRange) : IRequest<KpiResultDto>;
}
