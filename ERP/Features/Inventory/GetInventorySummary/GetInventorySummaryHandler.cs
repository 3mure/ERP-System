using BuildingBlocks.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Catalog_Service.Features.Inventory.GetInventorySummary
{
    public class GetInventorySummaryHandler
    : IRequestHandler<GetInventorySummaryQuery,InventorySummaryTableDto>
    {
        private readonly IBaseRepository<Entities.Inventory> _inventoryRepo;

        public GetInventorySummaryHandler(IBaseRepository<Entities.Inventory> inventoryRepo)
            => _inventoryRepo = inventoryRepo;

        public async Task<InventorySummaryTableDto> Handle(
            GetInventorySummaryQuery query,
            CancellationToken cancellationToken)
        {
            // Base query — filter by warehouse if not "all"
            var baseQuery = BuildBaseQuery(query.WarehouseId);

            // Load only the columns needed — avoid pulling full entities
            var rows = await baseQuery
                .Select(i => new
                {
                    i.CurrentStock,
                    i.DamagedStock,
                    i.ReservedStock,
                    CostPrice = i.Product.CostPrice,
                    LowStockThreshold = i.Product.MinStock
                })
                .ToListAsync(cancellationToken);

            // ── Calculate each summary box ────────────────────────

            // Box 1: Total physical units across all selected warehouses
            var totalQuantity = rows.Sum(r => r.CurrentStock);

            // Box 2: Total financial value = sum of (current_stock × cost_price)
            var totalValue = rows.Sum(r => r.CurrentStock * r.CostPrice);

            // Box 3: Products where stock > 0 but at or below the threshold
            //        (not out of stock yet, but needs reordering)
            var lowStockItems = rows.Count(r =>
                r.CurrentStock > 0 &&
                r.CurrentStock <= r.LowStockThreshold);

            // Box 4: Products completely out of stock
            var outOfStock = rows.Count(r => r.CurrentStock == 0);

            // Box 5: Total damaged/expired units logged across all warehouses
            var damagedExpired = rows.Sum(r => r.DamagedStock);

            return new InventorySummaryTableDto(
                TotalStockQuantity: totalQuantity.ToString("N0"),   // "14,500"
                TotalStockValue: $"EGP {totalValue:N0}",         // "EGP 250,000"
                LowStockItems: lowStockItems,
                OutOfStock: outOfStock,
                DamagedExpired: damagedExpired);
        }

        // ── Helper ───────────────────────────────────────────────
        private IQueryable<Entities.Inventory> BuildBaseQuery(string warehouseId)
        {
            var query = _inventoryRepo.Get(i => !i.IsDeleted);

            // If a specific warehouse is selected, filter to it only
            if (!string.IsNullOrWhiteSpace(warehouseId)
                && warehouseId.ToLower() != "all"
                && int.TryParse(warehouseId.Replace("WH-", ""), out var id))
            {
                query = query.Where(i => i.WarehouseId == id);
            }

            return query;
        }
    }

}
