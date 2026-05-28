using BuildingBlocks.Interfaces;
using Catalog_Service.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Catalog_Service.Features.Inventory.TransferStock
{
    public class TransferStockHandler
        : IRequestHandler<TransferStockCommand, TransferStockResult>
    {
        private readonly IBaseRepository<Entities.Inventory> _inventoryRepo;
        private readonly IBaseRepository<StockTransfer> _transferRepo;
        private readonly IBaseRepository<Product> _productsRepo;
        private readonly IBaseRepository<Warehouse> _warehouseRepo;
        private readonly IBaseRepository<User> _usersRepo;
        private readonly IUnitOfWork _unitOfWork;

        public TransferStockHandler(
            IBaseRepository<Entities.Inventory> inventoryRepo,
            IBaseRepository<StockTransfer> transferRepo,
            IBaseRepository<Product> productsRepo,
            IBaseRepository<Warehouse> warehouseRepo,
            IBaseRepository<User> usersRepo,
            IUnitOfWork unitOfWork)
        {
            _inventoryRepo = inventoryRepo;
            _transferRepo = transferRepo;
            _productsRepo = productsRepo;
            _warehouseRepo = warehouseRepo;
            _usersRepo = usersRepo;
            _unitOfWork = unitOfWork;
        }

        public async Task<TransferStockResult> Handle(
            TransferStockCommand command,
            CancellationToken cancellationToken)
        {
            var req = command.Request;

            // ════════════════════════════════════════════════════
            // PHASE 1 — VALIDATION (all checks before any DB write)
            // ════════════════════════════════════════════════════

            // ── 1a. Unique Reference Check ───────────────────────
            var referenceExists = await _transferRepo
                .Get(t => t.TransferReference == req.TransferReference)
                .AnyAsync(cancellationToken);

            if (referenceExists)
                return Fail($"Transfer Reference '{req.TransferReference}' already exists.");

            // ── 1b. Warehouse Validation ─────────────────────────
            if (req.FromWarehouseId == req.ToWarehouseId)
                return Fail("Source and destination warehouse cannot be the same.");

            var fromWarehouse = await _warehouseRepo
                .Get(w => w.Id == req.FromWarehouseId && !w.IsDeleted)
                .FirstOrDefaultAsync(cancellationToken);

            var toWarehouse = await _warehouseRepo
                .Get(w => w.Id == req.ToWarehouseId && !w.IsDeleted)
                .FirstOrDefaultAsync(cancellationToken);

            if (fromWarehouse == null) return Fail("Source warehouse not found.");
            if (toWarehouse == null) return Fail("Destination warehouse not found.");

            // ── 1c. Resolve all products by Int ID ───────────────
            var productIds = req.Items.Select(i => i.ProductId).Distinct().ToList();

            var products = await _productsRepo
                .Get(p => productIds.Contains(p.Id) && !p.IsDeleted)
                .ToListAsync(cancellationToken);

            var productLookup = products.ToDictionary(p => p.Id);

            // ── 1d. Load Inventory rows ───────────────────────────
            var sourceInventories = await _inventoryRepo
                .Get(i => i.WarehouseId == req.FromWarehouseId && productIds.Contains(i.ProductId) && !i.IsDeleted)
                .ToListAsync(cancellationToken);
            var sourceInventoryLookup = sourceInventories.ToDictionary(i => i.ProductId);

            var destInventories = await _inventoryRepo
                .Get(i => i.WarehouseId == req.ToWarehouseId && productIds.Contains(i.ProductId) && !i.IsDeleted)
                .ToListAsync(cancellationToken);
            var destInventoryLookup = destInventories.ToDictionary(i => i.ProductId);

            // ── 1e. MIN/MAX INTEGRITY & AVAILABILITY CHECK ────────
            foreach (var item in req.Items)
            {
                if (!productLookup.TryGetValue(item.ProductId, out var product))
                    return Fail($"Product ID '{item.ProductId}' not found in catalog.");

                if (!sourceInventoryLookup.TryGetValue(product.Id, out var srcInv))
                    return Fail($"Product '{product.Name}' has no inventory record in source warehouse.");

                var available = srcInv.CurrentStock - srcInv.ReservedStock;

                if (available < item.Quantity)
                    return Fail($"Insufficient stock in source warehouse for '{product.Name}'. Available: {available}, Requested: {item.Quantity}.");

                if ((srcInv.CurrentStock - item.Quantity) < product.MinStock)
                    return Fail($"Transfer denied: Source stock for '{product.Name}' would fall below MinStock ({product.MinStock}).");

                var currentDestStock = destInventoryLookup.TryGetValue(product.Id, out var dstInv) ? dstInv.CurrentStock : 0;

                if ((currentDestStock + item.Quantity) > product.MaxStock)
                    return Fail($"Transfer denied: Destination stock for '{product.Name}' would exceed MaxStock ({product.MaxStock}).");
            }

            // ── 1f. Projection: Get User Name cleanly ─────────────
            var authorizedUserName = await _usersRepo
                .Get(u => u.Id == req.AuthorizedById && !u.IsDeleted)
                .Select(u => u.Name)
                .FirstOrDefaultAsync(cancellationToken) ?? "Unknown System User";

            // ════════════════════════════════════════════════════
            // PHASE 2 — DATABASE WRITES INSIDE A TRANSACTION
            // ════════════════════════════════════════════════════
            await _unitOfWork.BeginTransactionAsync();

            try
            {
                var manifestItems = new List<ManifestItem>();
                var transferItems = new List<StockTransferItem>();

                foreach (var item in req.Items)
                {
                    var product = productLookup[item.ProductId];
                    var srcInv = sourceInventoryLookup[product.Id];

                    // ── Deduct from source ────────────────────────
                    srcInv.CurrentStock -= item.Quantity;
                    srcInv.LastUpdate = DateTime.UtcNow;
                    _inventoryRepo.Update(srcInv);

                    // ── Add to or Create destination ──────────────
                    if (destInventoryLookup.TryGetValue(product.Id, out var dstInv))
                    {
                        dstInv.CurrentStock += item.Quantity;
                        dstInv.LastUpdate = DateTime.UtcNow;
                        _inventoryRepo.Update(dstInv);
                    }
                    else
                    {
                        var newInventoryRow = new Entities.Inventory
                        {
                            ProductId = product.Id,
                            WarehouseId = req.ToWarehouseId,
                            CurrentStock = item.Quantity,
                            ReservedStock = 0,
                            DamagedStock = 0,
                            LastUpdate = DateTime.UtcNow
                        };
                        await _inventoryRepo.AddAsync(newInventoryRow);
                        destInventoryLookup[product.Id] = newInventoryRow;
                    }

                    // ── Build transfer line items ─────────────────
                    transferItems.Add(new StockTransferItem
                    {
                        ProductId = product.Id,
                        Quantity = item.Quantity
                    });

                    manifestItems.Add(new ManifestItem(
                        product.ProductCode,
                        product.Name,
                        item.Quantity));
                }

                // ── Save Parent & Children Together ───────────────
                var transfer = new StockTransfer
                {
                    TransferReference = req.TransferReference,
                    FromWarehouseId = req.FromWarehouseId,
                    ToWarehouseId = req.ToWarehouseId,
                    AuthorizedBy = req.AuthorizedById,
                    Notes = req.Notes,
                    Status = "completed",
                    CreatedAt = DateTime.UtcNow,
                    Items = transferItems // EF Core automatically maps the TransferId to these items
                };

                await _transferRepo.AddAsync(transfer);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                // ── COMMIT ────────────────────────────────────────
                await _unitOfWork.CommitTransactionAsync();

                // ── Build final response ──────────────────────────
                var manifest = new ManifestDto(
                    req.TransferReference,
                    fromWarehouse.Name,
                    toWarehouse.Name,
                    authorizedUserName,
                    DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm"),
                    req.Notes ?? string.Empty,
                    manifestItems);

                return new TransferStockResult(
                    Success: true,
                    Message: "Stock transferred successfully.",
                    TransferReference: req.TransferReference,
                    Manifest: manifest);
            }
            catch (Exception ex)
            {
                // ── ROLLBACK ──────────────────────────────────────
                await _unitOfWork.RollbackTransactionAsync();
                return Fail($"Transfer failed and was rolled back. Reason: {ex.Message}");
            }
        }

        // ── Helpers ──────────────────────────────────────────────
        private static TransferStockResult Fail(string message) =>
            new(Success: false, Message: message);
    }
}