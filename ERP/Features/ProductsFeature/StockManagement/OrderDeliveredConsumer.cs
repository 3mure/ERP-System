using BuildingBlocks.IntegrationEvents;
using Catalog_Service.Messaging.Inventory;
using MassTransit;

namespace Catalog_Service.Features.ProductsFeature.StockManagement;

/// <summary>
/// Finalizes stock on delivery. If stock was only reserved, commits deduction.
/// Idempotent when CommitStockEvent already ran on payment.
/// </summary>
public class OrderDeliveredConsumer : IConsumer<OrderDeliveredEvent>
{
    private readonly IInventoryStockService _stock;
    private readonly ILogger<OrderDeliveredConsumer> _logger;

    public OrderDeliveredConsumer(
        IInventoryStockService stock,
        ILogger<OrderDeliveredConsumer> logger)
    {
        _stock = stock;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<OrderDeliveredEvent> context)
    {
        var msg = context.Message;
        _logger.LogInformation("OrderDelivered {OrderId}, {Count} items", msg.OrderId, msg.Items.Count);

        if (msg.Items.Count == 0)
            return;

        var branchId = msg.BranchId > 0 ? msg.BranchId : 8001;
        var items = msg.Items.Select(i => new StockEventItemDto(i.ProductId, i.Quantity)).ToList();

        var result = await _stock.CommitAsync(branchId, msg.WarehouseId, items, context.CancellationToken);
        if (!result.Success)
        {
            _logger.LogWarning(
                "OrderDelivered stock commit skipped/failed for {OrderId}: {Error}",
                msg.OrderId, result.ErrorMessage);
        }
    }
}
