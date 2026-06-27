using BuildingBlocks.Interfaces;
using Catalog_Service.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Catalog_Service.Features.ProductPerformanceAndGetLowMovementStock.GetLowMovementStock
{
    public class GetLowMovementStockHandler
    : IRequestHandler<GetLowMovementStockQuery, List<LowMovementStockDto>>
    {
        private readonly IBaseRepository<Product> _productsRepo;
        private readonly IBaseRepository<SalesOrderItem> _itemsRepo;
        private readonly IBaseRepository<SalesOrder> _ordersRepo;
        private readonly IBaseRepository<Entities.Inventory> _inventoryRepo;

        public GetLowMovementStockHandler(
            IBaseRepository<Product> productsRepo,
            IBaseRepository<SalesOrderItem> itemsRepo,
            IBaseRepository<SalesOrder> ordersRepo,
            IBaseRepository<Entities.Inventory> inventoryRepo)
        {
            _productsRepo = productsRepo;
            _itemsRepo = itemsRepo;
            _ordersRepo = ordersRepo;
            _inventoryRepo = inventoryRepo;
        }

        public async Task<List<LowMovementStockDto>> Handle(
            GetLowMovementStockQuery query,
            CancellationToken cancellationToken)
        {
            var today = DateTime.UtcNow.Date;

            // Step 1: Get all active products that have stock somewhere
            //         We only care about products sitting on the shelf
            var productsWithStock = await _inventoryRepo
                .Get(i => !i.IsDeleted && i.CurrentStock > 0)
                .Select(i => new { i.ProductId, i.CurrentStock })
                .ToListAsync(cancellationToken);

            if (!productsWithStock.Any())
                return new List<LowMovementStockDto>();

            // Group inventory by product → total stock across all warehouses
            var stockByProduct = productsWithStock
                .GroupBy(i => i.ProductId)
                .ToDictionary(g => g.Key, g => g.Sum(i => i.CurrentStock));

            var productIds = stockByProduct.Keys.ToList();

            // Step 2: Get product details (name + created date)
            var products = await _productsRepo
                .Get(p => productIds.Contains(p.Id) && !p.IsDeleted && p.IsActive)
                .Select(p => new { p.Id, p.Name, p.CreatedAt })
                .ToListAsync(cancellationToken);

            // Step 3: Get IDs of completed orders
            var completedOrderIds = await _ordersRepo
                .Get(o => o.Status == "Completed" && !o.IsDeleted)
                .Select(o => o.Id)
                .ToListAsync(cancellationToken);

            // Step 4: For each product, find the date of its LAST completed sale
            //         Only pull the columns we need — no full entity load
            var lastSaleDates = await _itemsRepo
                .Get(i => productIds.Contains(i.ProductId)
                       && completedOrderIds.Contains(i.OrderId)
                       && !i.IsDeleted)
                .GroupBy(i => i.ProductId)
                .Select(g => new
                {
                    ProductId = g.Key,
                    LastSaleDate = g.Max(i => i.SalesOrder.OrderDate)
                })
                .ToListAsync(cancellationToken);

            // Build a quick lookup: ProductId → LastSaleDate
            var lastSaleLookup = lastSaleDates
                .ToDictionary(x => x.ProductId, x => x.LastSaleDate);

            // Step 5: Calculate days without sale for each product
            //         If never sold → use the product's CreatedAt as the start date
            var result = products
                .Select(p =>
                {
                    var lastSale = lastSaleLookup.TryGetValue(p.Id, out var date)
                        ? date
                        : p.CreatedAt;                          // never sold → use creation date

                    var daysWithoutSale = (today - lastSale.Date).Days;
                    var totalStock = stockByProduct[p.Id];

                    return new
                    {
                        p.Name,
                        DaysWithoutSale = daysWithoutSale,
                        TotalStock = totalStock
                    };
                })
                .OrderByDescending(x => x.DaysWithoutSale)     // longest sitting first
                .Take(5)
                .Select(x => new LowMovementStockDto(
                    Name: x.Name,
                    DaysWithoutSale: x.DaysWithoutSale,
                    TotalStock: x.TotalStock))
                .ToList();

            return result;
        }
    }
}
