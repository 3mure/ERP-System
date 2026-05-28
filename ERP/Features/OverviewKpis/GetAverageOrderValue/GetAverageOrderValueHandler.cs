using BuildingBlocks.Interfaces;
using Catalog_Service.Entities;
using Catalog_Service.Features.OverviewKpis.GetNetProfit;
using Dashboard_Service.Features.OverviewKpis;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Catalog_Service.Features.OverviewKpis.GetAverageOrderValue
{
    public class GetAverageOrderValueHandler
     : IRequestHandler<GetAverageOrderValueQuery, KpiResultDto>
    {
        private readonly IBaseRepository<SalesOrder> _ordersRepo;

        public GetAverageOrderValueHandler(IBaseRepository<SalesOrder> ordersRepo)
            => _ordersRepo = ordersRepo;

        public async Task<KpiResultDto> Handle(
            GetAverageOrderValueQuery query, CancellationToken cancellationToken)
        {
            var (currentStart, currentEnd, prevStart, prevEnd) =
                DateRangeHelper.Resolve(query.DateRange);

            var current = await CalcAOV(currentStart, currentEnd, cancellationToken);
            var previous = await CalcAOV(prevStart, prevEnd, cancellationToken);

            var (trendText, status) = TrendHelper.Calc(current, previous);

            return new KpiResultDto(
                Value: FormatHelper.FormatEGP(current),
                Trend: $"{trendText} vs Last period",
                Status: status);
        }

        private async Task<decimal> CalcAOV(DateTime start, DateTime end, CancellationToken ct)
        {
            var orders = await _ordersRepo
                .Get(o => o.Status == "Completed"
                       && !o.IsDeleted
                       && o.OrderDate >= start
                       && o.OrderDate <= end)
                .Select(o => o.TotalAmount)
                .ToListAsync(ct);

            if (!orders.Any()) return 0;

            return orders.Average();   // Total ÷ Count
        }
    }

}
