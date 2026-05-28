using BuildingBlocks.IntegrationEvents;
using BuildingBlocks.Interfaces;
using Catalog_Service.Entities;
using Microsoft.EntityFrameworkCore;

namespace Catalog_Service.Messaging.Inventory;

using InventoryEntity = Catalog_Service.Entities.Inventory;

public class InventoryStockService : IInventoryStockService
{
    private readonly IBaseRepository<InventoryEntity> _inventoryRepo;
    private readonly IBaseRepository<Product> _productRepo;
    private readonly IBaseRepository<Warehouse> _warehouseRepo;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<InventoryStockService> _logger;

    public InventoryStockService(
        IBaseRepository<InventoryEntity> inventoryRepo,
        IBaseRepository<Product> productRepo,
        IBaseRepository<Warehouse> warehouseRepo,
        IUnitOfWork unitOfWork,
        ILogger<InventoryStockService> logger)
    {
        _inventoryRepo = inventoryRepo;
        _productRepo = productRepo;
        _warehouseRepo = warehouseRepo;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public Task<StockOperationResult> ReserveAsync(
        int branchId,
        int? warehouseId,
        IReadOnlyList<StockEventItemDto> items,
        CancellationToken cancellationToken = default)
        => ApplyAsync(branchId, warehouseId, items, (inv, qty) =>
        {
            var available = inv.CurrentStock - inv.ReservedStock;
            if (available < qty)
                return $"Insufficient stock for product {inv.ProductId}. Available: {available}, requested: {qty}.";

            inv.ReservedStock += qty;
            return null;
        }, cancellationToken);

    public Task<StockOperationResult> ReleaseReservationAsync(
        int branchId,
        int? warehouseId,
        IReadOnlyList<StockEventItemDto> items,
        CancellationToken cancellationToken = default)
        => ApplyAsync(branchId, warehouseId, items, (inv, qty) =>
        {
            inv.ReservedStock = Math.Max(0, inv.ReservedStock - qty);
            return null;
        }, cancellationToken);

    public Task<StockOperationResult> CommitAsync(
        int branchId,
        int? warehouseId,
        IReadOnlyList<StockEventItemDto> items,
        CancellationToken cancellationToken = default)
        => ApplyAsync(branchId, warehouseId, items, (inv, qty) =>
        {
            if (inv.CurrentStock < qty)
                return $"Insufficient current stock for product {inv.ProductId}.";

            inv.CurrentStock -= qty;
            inv.ReservedStock = Math.Max(0, inv.ReservedStock - qty);
            return null;
        }, cancellationToken);

    private async Task<StockOperationResult> ApplyAsync(
        int branchId,
        int? warehouseId,
        IReadOnlyList<StockEventItemDto> items,
        Func<InventoryEntity, int, string?> apply,
        CancellationToken cancellationToken)
    {
        if (items.Count == 0)
            return new StockOperationResult(true, null);

        var warehouseIds = await ResolveWarehouseIdsAsync(branchId, warehouseId, cancellationToken);
        if (warehouseIds.Count == 0)
            return new StockOperationResult(false, $"No warehouse found for branch {branchId}.");

        foreach (var item in items)
        {
            var inv = await FindInventoryAsync(item.ProductId, warehouseIds, cancellationToken);
            if (inv is null)
                return new StockOperationResult(false, $"No inventory for product {item.ProductId} in branch {branchId}.");

            var error = apply(inv, item.Quantity);
            if (error is not null)
                return new StockOperationResult(false, error);

            inv.LastUpdate = DateTime.UtcNow;
            _inventoryRepo.Update(inv);

            await SyncProductStockQuantityAsync(inv.ProductId, cancellationToken);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return new StockOperationResult(true, null);
    }

    private async Task<List<int>> ResolveWarehouseIdsAsync(
        int branchId,
        int? warehouseId,
        CancellationToken cancellationToken)
    {
        if (warehouseId.HasValue)
            return [warehouseId.Value];

        return await _warehouseRepo
            .Get(w => w.BranchId == branchId && !w.IsDeleted)
            .Select(w => w.Id)
            .ToListAsync(cancellationToken);
    }

    private async Task<InventoryEntity?> FindInventoryAsync(
        int productId,
        List<int> warehouseIds,
        CancellationToken cancellationToken)
    {
        return await _inventoryRepo
            .Get(i => i.ProductId == productId
                   && warehouseIds.Contains(i.WarehouseId)
                   && !i.IsDeleted)
            .OrderByDescending(i => i.CurrentStock - i.ReservedStock)
            .FirstOrDefaultAsync(cancellationToken);
    }

    private async Task SyncProductStockQuantityAsync(int productId, CancellationToken cancellationToken)
    {
        var totalAvailable = await _inventoryRepo
            .Get(i => i.ProductId == productId && !i.IsDeleted)
            .SumAsync(i => i.CurrentStock - i.ReservedStock, cancellationToken);

        var product = await _productRepo.GetByIdAsync(productId);
        if (product is null) return;

        product.StockQuantity = Math.Max(0, totalAvailable);
        product.UpdatedAt = DateTime.UtcNow;
        _productRepo.Update(product);
    }
}
