using BuildingBlocks.Interfaces;
using Catalog_Service.Entities;
using Dashboard_Service.Features.OverviewKpis;
using MediatR;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.EntityFrameworkCore;

namespace Catalog_Service.Features.OverviewKpis.GetNetProfit
{
    public class GetNetProfitHandler : IRequestHandler<GetNetProfitQuery, KpiResultDto>
    {
        private readonly IBaseRepository<SalesOrder> _ordersRepo;
        private readonly IBaseRepository<SalesOrderItem> _itemsRepo;

        public GetNetProfitHandler(
            IBaseRepository<SalesOrder> ordersRepo,
            IBaseRepository<SalesOrderItem> itemsRepo)
        {
            _ordersRepo = ordersRepo;
            _itemsRepo = itemsRepo;
        }

        public async Task<KpiResultDto> Handle(
            GetNetProfitQuery query, CancellationToken cancellationToken)
        {
            var (currentStart, currentEnd, prevStart, prevEnd) =
                DateRangeHelper.Resolve(query.DateRange);

            var current = await CalcNetProfit(currentStart, currentEnd, cancellationToken);
            var previous = await CalcNetProfit(prevStart, prevEnd, cancellationToken);

            var (trendText, status) = TrendHelper.Calc(current, previous);

            return new KpiResultDto(
                Value: FormatHelper.FormatEGP(current),
                Trend: $"{trendText} vs Last period",
                Status: status);
        }
      private async Task<decimal> CalcNetProfit(
        DateTime start, DateTime end, CancellationToken ct)
        {
            // Get order IDs for completed orders in range
            var orderIds = await _ordersRepo
                .Get(o => o.Status == "Completed"
                       && !o.IsDeleted
                       && o.OrderDate >= start
                       && o.OrderDate <= end)
                .Select(o => o.Id)
                .ToListAsync(ct);

            if (!orderIds.Any()) return 0;

            // Sum revenue and cost from line items
            var items = await _itemsRepo
                .Get(i => orderIds.Contains(i.OrderId) && !i.IsDeleted)
                .Select(i => new { i.Quantity, i.UnitPrice, i.CostPrice })
                .ToListAsync(ct);

            var revenue = items.Sum(i => i.Quantity * i.UnitPrice);
            var cost = items.Sum(i => i.Quantity * i.CostPrice);

            return (decimal)revenue - (decimal)cost;
        }
    }
}
