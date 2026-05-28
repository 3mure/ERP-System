using BuildingBlocks.IntegrationEvents;
using Catalog_Service.Messaging.Inventory;
using MassTransit;

namespace Catalog_Service.Features.ProductsFeature.StockManagement;

public class PaymentFailedConsumer : IConsumer<PaymentFailedEvent>
{
    private readonly IInventoryStockService _stock;
    private readonly ILogger<PaymentFailedConsumer> _logger;

    public PaymentFailedConsumer(IInventoryStockService stock, ILogger<PaymentFailedConsumer> logger)
    {
        _stock = stock;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<PaymentFailedEvent> context)
    {
        var msg = context.Message;
        _logger.LogInformation(
            "PaymentFailed Order {OrderId}: {Error}",
            msg.OrderId, msg.ErrorMessage);

        if (msg.Items.Count == 0)
            return;

        var branchId = msg.BranchId > 0 ? msg.BranchId : 8001;
        var items = msg.Items.Select(i => new StockEventItemDto(i.ProductId, i.Quantity)).ToList();

        var result = await _stock.ReleaseReservationAsync(
            branchId, msg.WarehouseId, items, context.CancellationToken);

        if (!result.Success)
            _logger.LogWarning("PaymentFailed release failed for {OrderId}: {Error}", msg.OrderId, result.ErrorMessage);
    }
}
