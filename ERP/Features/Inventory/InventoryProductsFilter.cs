using Catalog_Service.Features.Inventory.GetInventoryProducts;

namespace Catalog_Service.Features.Inventory
{
    public static class InventoryProductsFilter
    {
        public static IQueryable<Entities.Inventory> Apply(
            IQueryable<Entities.Inventory> query,
            string? search,
            string warehouseId,
            string status)
        {
            // Filter by warehouse
            if (!string.IsNullOrWhiteSpace(warehouseId)
                && warehouseId.ToLower() != "all"
                && int.TryParse(warehouseId.Replace("WH-", ""), out var wId))
            {
                query = query.Where(i => i.WarehouseId == wId);
            }

            // Filter by product name search
            if (!string.IsNullOrWhiteSpace(search))
            {
                var s = search.ToLower();
                query = query.Where(i => i.Product.Name.ToLower().Contains(s));
            }

            // Filter by status
            if (!string.IsNullOrWhiteSpace(status) && status.ToLower() != "all")
            {
                query = status.ToLower() switch
                {
                    "in_stock" => query.Where(i =>
                                        i.CurrentStock > i.Product.MinStock),
                    "low_stock" => query.Where(i =>
                                        i.CurrentStock > 0 &&
                                        i.CurrentStock <= i.Product.MinStock),
                    "out_of_stock" => query.Where(i => i.CurrentStock == 0),
                    _ => query
                };
            }

            return query;
        }

        /// <summary>Projects a raw Inventory row into the display DTO.</summary>
        public static InventoryProductRowDto ToDto(Entities.Inventory i) =>
            new(
                ProductId: i.Product.ProductCode,
                ProductName: i.Product.Name,
                Category: i.Product.Category?.Name ?? "Uncategorized",
                BranchWarehouse: i.Warehouse.Name,
                CurrentStock: i.CurrentStock,
                ReservedStock: i.ReservedStock,
                AvailableStock: i.CurrentStock - i.ReservedStock,
                Status: GetStatus(i),
                StatusColor: GetStatusColor(i),
                LastUpdate: i.LastUpdate.ToString("yyyy-MM-dd")
            );

        public static string GetStatus(Entities.Inventory i) =>
            i.CurrentStock == 0 ? "Out of Stock" :
            i.CurrentStock <= i.Product.MinStock ? "Low Stock" :
                                                               "In Stock";

        public static string GetStatusColor(Entities.Inventory i) =>
            i.CurrentStock == 0 ? "red" :
            i.CurrentStock <= i.Product.MinStock ? "yellow" :
                                                               "green";
    }
}
