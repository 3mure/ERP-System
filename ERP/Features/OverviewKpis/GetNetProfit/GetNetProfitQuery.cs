using MediatR;

namespace Catalog_Service.Features.OverviewKpis.GetNetProfit
{
    public record GetNetProfitQuery(string DateRange) : IRequest<KpiResultDto>;
}
