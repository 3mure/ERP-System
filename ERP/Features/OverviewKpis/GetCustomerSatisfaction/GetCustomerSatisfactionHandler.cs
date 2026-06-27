using BuildingBlocks.Interfaces;
using Catalog_Service.Entities;
using Catalog_Service.Features.OverviewKpis.GetNetProfit;
using Dashboard_Service.Features.OverviewKpis;
using MediatR;

namespace Catalog_Service.Features.OverviewKpis.GetCustomerSatisfaction
{
    public class GetCustomerSatisfactionHandler
     : IRequestHandler<GetCustomerSatisfactionQuery, KpiResultDto>
    {
        private readonly IBaseRepository<SalesOrder> _ordersRepo;

        public GetCustomerSatisfactionHandler(IBaseRepository<SalesOrder> ordersRepo)
            => _ordersRepo = ordersRepo;

        public async Task<KpiResultDto> Handle(
            GetCustomerSatisfactionQuery query, CancellationToken cancellationToken)
        {
            var (currentStart, currentEnd, prevStart, prevEnd) =
                DateRangeHelper.Resolve(query.DateRange);

            var current = await CalcSatisfaction(currentStart, currentEnd, cancellationToken);
            var previous = await CalcSatisfaction(prevStart, prevEnd, cancellationToken);

            var (trendText, status) = TrendHelper.Calc(current, previous);

            return new KpiResultDto(
                Value: $"{current:F0}%",              // e.g. "94%"
                Trend: $"{trendText} vs Last period",
                Status: status);
        }

        private async Task<decimal> CalcSatisfaction(
            DateTime start, DateTime end, CancellationToken ct)
        {
            // Total finalized orders (completed + returned — excludes pending/cancelled)
            var totalFinalized = await _ordersRepo.CountAsync(
                o => !o.IsDeleted
                  && o.OrderDate >= start
                  && o.OrderDate <= end
                  && (o.Status == "Completed" || o.Status == "Returned"));

            if (totalFinalized == 0) return 0;

            // Satisfied = completed without a return
            var satisfied = await _ordersRepo.CountAsync(
                o => o.Status == "Completed"
                  && !o.IsDeleted
                  && o.OrderDate >= start
                  && o.OrderDate <= end);

            return ((decimal)satisfied / totalFinalized) * 100;
        }
    }

}
