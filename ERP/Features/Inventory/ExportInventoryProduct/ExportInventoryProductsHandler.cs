using BuildingBlocks.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace Catalog_Service.Features.Inventory.ExportInventoryProduct
{
    public class ExportInventoryProductsHandler
    : IRequestHandler<ExportInventoryProductsQuery, byte[]>
    {
        private readonly IBaseRepository<Entities.Inventory> _inventoryRepo;

        public ExportInventoryProductsHandler(IBaseRepository<Entities.Inventory> inventoryRepo)
            => _inventoryRepo = inventoryRepo;

        public async Task<byte[]> Handle(
            ExportInventoryProductsQuery query,
            CancellationToken cancellationToken)
        {
            var baseQuery = InventoryProductsFilter.Apply(
                _inventoryRepo.Get(i => !i.IsDeleted && i.Product.IsActive),
                query.Search,
                query.WarehouseId,
                query.Status);

            var rows = await baseQuery
                .OrderBy(i => i.Product.Name)
                .Include(i => i.Product).ThenInclude(p => p.Category)
                .Include(i => i.Warehouse)
                .ToListAsync(cancellationToken);

            var dtos = rows.Select(InventoryProductsFilter.ToDto).ToList();

            // Build CSV
            var sb = new StringBuilder();

            // Header row
            sb.AppendLine(
                "Product ID,Product Name,Category,Branch/Warehouse," +
                "Current Stock,Reserved Stock,Available Stock,Status,Last Update");

            // Data rows
            foreach (var r in dtos)
            {
                sb.AppendLine(
                    $"{r.ProductId}," +
                    $"\"{r.ProductName}\"," +       // wrap in quotes to handle commas in names
                    $"\"{r.Category}\"," +
                    $"\"{r.BranchWarehouse}\"," +
                    $"{r.CurrentStock}," +
                    $"{r.ReservedStock}," +
                    $"{r.AvailableStock}," +
                    $"{r.Status}," +
                    $"{r.LastUpdate}");
            }

            return Encoding.UTF8.GetBytes(sb.ToString());
        }
    }

}
