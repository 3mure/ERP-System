using Catalog_Service.Features.OverviewKpis.GetNetProfit;
using MediatR;

namespace Catalog_Service.Features.OverviewKpis.GetTotalSales
{
    public record GetTotalSalesQuery(string DateRange) : IRequest<KpiResultDto>;
}
