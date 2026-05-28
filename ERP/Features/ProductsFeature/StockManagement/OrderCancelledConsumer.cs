using BuildingBlocks.IntegrationEvents;
using Catalog_Service.Messaging.Inventory;
using MassTransit;

namespace Catalog_Service.Features.ProductsFeature.StockManagement;

public class OrderCancelledConsumer : IConsumer<OrderCancelledEvent>
{
    private readonly IInventoryStockService _stock;
    private readonly ILogger<OrderCancelledConsumer> _logger;

    public OrderCancelledConsumer(IInventoryStockService stock, ILogger<OrderCancelledConsumer> logger)
    {
        _stock = stock;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<OrderCancelledEvent> context)
    {
        var msg = context.Message;
        _logger.LogInformation(
            "OrderCancelled {OrderId}: {Reason}",
            msg.OrderId, msg.CancellationReason);

        if (msg.Items.Count == 0)
            return;

        var branchId = msg.BranchId > 0 ? msg.BranchId : 8001;
        var items = msg.Items.Select(i => new StockEventItemDto(i.ProductId, i.Quantity)).ToList();

        var result = await _stock.ReleaseReservationAsync(
            branchId, msg.WarehouseId, items, context.CancellationToken);

        if (!result.Success)
            _logger.LogWarning("OrderCancelled release failed for {OrderId}: {Error}", msg.OrderId, result.ErrorMessage);
    }
}
