using BuildingBlocks.Interfaces;
using Catalog_Service.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Catalog_Service.Features.ProductPerformanceAndGetLowMovementStock.GetTopSellingProducts
{
    public class GetTopSellingProductsHandler
     : IRequestHandler<GetTopSellingProductsQuery, List<TopSellingProductDto>>
    {
        private readonly IBaseRepository<SalesOrderItem> _itemsRepo;
        private readonly IBaseRepository<SalesOrder> _ordersRepo;

        public GetTopSellingProductsHandler(
            IBaseRepository<SalesOrderItem> itemsRepo,
            IBaseRepository<SalesOrder> ordersRepo)
        {
            _itemsRepo = itemsRepo;
            _ordersRepo = ordersRepo;
        }

        public async Task<List<TopSellingProductDto>> Handle(
            GetTopSellingProductsQuery query,
            CancellationToken cancellationToken)
        {
            // Step 1: Get IDs of all completed orders
            var completedOrderIds = await _ordersRepo
                .Get(o => o.Status == "Completed" && !o.IsDeleted)
                .Select(o => o.Id)
                .ToListAsync(cancellationToken);

            if (!completedOrderIds.Any())
                return new List<TopSellingProductDto>();

            // Step 2: Get all line items from those orders
            //         Include Product name via navigation property
            var soldItems = await _itemsRepo
                .Get(i => completedOrderIds.Contains(i.OrderId) && !i.IsDeleted)
                .Select(i => new
                {
                    ProductName = i.Product.Name,
                    i.Quantity
                })
                .ToListAsync(cancellationToken);

            // Step 3: Grand total of ALL units sold across every product
            //         Used as the denominator for percentage calculation
            var grandTotalUnits = soldItems.Sum(i => i.Quantity);

            if (grandTotalUnits == 0)
                return new List<TopSellingProductDto>();

            // Step 4: Group by product → sum → sort → take top 5 → calc %
            var result = soldItems
                .GroupBy(i => i.ProductName)
                .Select(g => new
                {
                    Name = g.Key,
                    SoldUnits = g.Sum(i => i.Quantity)
                })
                .OrderByDescending(x => x.SoldUnits)
                .Take(5)
                .Select(x => new TopSellingProductDto(
                    Name: x.Name,
                    SoldUnits: x.SoldUnits,
                    Percentage: $"{((decimal)x.SoldUnits / grandTotalUnits * 100):F1}%"))
                .ToList();

            return result;
        }
    }
}
