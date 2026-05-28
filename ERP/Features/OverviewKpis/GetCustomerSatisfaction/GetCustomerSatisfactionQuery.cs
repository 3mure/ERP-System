using Catalog_Service.Features.OverviewKpis.GetNetProfit;
using MediatR;

namespace Catalog_Service.Features.OverviewKpis.GetCustomerSatisfaction
{
    public record GetCustomerSatisfactionQuery(string DateRange) : IRequest<KpiResultDto>;

}
