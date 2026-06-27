using BuildingBlocks.Interfaces;
using Catalog_Service.Entities;
using Catalog_Service.Features.OverviewKpis.GetNetProfit;
using Dashboard_Service.Features.OverviewKpis;
using MediatR;

namespace Catalog_Service.Features.OverviewKpis.GetTotalSales
{
    public class GetTotalSalesHandler : IRequestHandler<GetTotalSalesQuery, KpiResultDto>
    {
        private readonly IBaseRepository<SalesOrder> _ordersRepo;

        public GetTotalSalesHandler(IBaseRepository<SalesOrder> ordersRepo)
            => _ordersRepo = ordersRepo;

        public async Task<KpiResultDto> Handle(
            GetTotalSalesQuery query, CancellationToken cancellationToken)
        {
            var (currentStart, currentEnd, prevStart, prevEnd) =
                DateRangeHelper.Resolve(query.DateRange);

            var current = await Count(currentStart, currentEnd, cancellationToken);
            var previous = await Count(prevStart, prevEnd, cancellationToken);

            var (trendText, status) = TrendHelper.Calc(current, previous);

            return new KpiResultDto(
                Value: current.ToString("N0"),          // e.g. "1,340"
                Trend: $"{trendText} vs Last period",
                Status: status);
        }

        private async Task<decimal> Count(DateTime start, DateTime end, CancellationToken ct)
        {
            var count = await _ordersRepo.CountAsync(
                o => o.Status == "Completed"
                  && !o.IsDeleted
                  && o.OrderDate >= start
                  && o.OrderDate <= end);

            return count;
        }
    }
}
