using BuildingBlocks.Interfaces;
using Catalog_Service.Entities;
using Catalog_Service.Features.OverviewKpis.GetNetProfit;
using Dashboard_Service.Features.OverviewKpis;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Catalog_Service.Features.OverviewKpis.GetStockTurnoverRate
{
    public class GetStockTurnoverRateHandler
     : IRequestHandler<GetStockTurnoverRateQuery, KpiResultDto>
    {
        private readonly IBaseRepository<SalesOrderItem> _itemsRepo;
        private readonly IBaseRepository<SalesOrder> _ordersRepo;
        private readonly IBaseRepository<Entities.Inventory> _inventoryRepo;

        public GetStockTurnoverRateHandler(
            IBaseRepository<SalesOrderItem> itemsRepo,
            IBaseRepository<SalesOrder> ordersRepo,
            IBaseRepository<Entities.Inventory> inventoryRepo)
        {
            _itemsRepo = itemsRepo;
            _ordersRepo = ordersRepo;
            _inventoryRepo = inventoryRepo;
        }

        public async Task<KpiResultDto> Handle(
            GetStockTurnoverRateQuery query, CancellationToken cancellationToken)
        {
            var (currentStart, currentEnd, prevStart, prevEnd) =
                DateRangeHelper.Resolve(query.DateRange);

            var current = await CalcTurnover(currentStart, currentEnd, cancellationToken);
            var previous = await CalcTurnover(prevStart, prevEnd, cancellationToken);

            var (trendText, status) = TrendHelper.Calc(current, previous);

            return new KpiResultDto(
                Value: $"{current:F1}x",              // e.g. "6.2x"
                Trend: $"{trendText} vs Last period",
                Status: status);
        }

        private async Task<decimal> CalcTurnover(
            DateTime start, DateTime end, CancellationToken ct)
        {
            // Step 1: Get completed order IDs in range
            var orderIds = await _ordersRepo
                .Get(o => o.Status == "Completed"
                       && !o.IsDeleted
                       && o.OrderDate >= start
                       && o.OrderDate <= end)
                .Select(o => o.Id)
                .ToListAsync(ct);

            if (!orderIds.Any()) return 0;

            // Step 2: Cost of Goods Sold = sum of (qty × cost price) for those orders
            var cogs = await _itemsRepo
                .Get(i => orderIds.Contains(i.OrderId) && !i.IsDeleted)
                .SumAsync(i => i.Quantity * i.CostPrice, ct);

            // Step 3: Average Inventory Value = sum of (current_stock × cost_price) across all warehouses
            var avgInventoryValue = await _inventoryRepo
                .Get(i => !i.IsDeleted)
                .SumAsync(i => i.CurrentStock * i.Product.CostPrice, ct);

            if (avgInventoryValue == 0) return 0;

            return (decimal)cogs / (decimal)avgInventoryValue;
        }
    }
}
