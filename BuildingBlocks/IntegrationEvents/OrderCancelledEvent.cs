namespace BuildingBlocks.IntegrationEvents
{
    /// <summary>
    /// Event published when an order is cancelled.
    /// Consumed by Catalog Service to restore reserved stock.
    /// </summary>
    public record OrderCancelledEvent(
        int OrderId,
        string UserId,
        string CancellationReason,
        DateTime CancelledAt,
        List<OrderCancelledItemDto> Items,
        int BranchId = 0,
        int? WarehouseId = null
    );

    public record OrderCancelledItemDto(
        int ProductId,
        int Quantity
    );
}
