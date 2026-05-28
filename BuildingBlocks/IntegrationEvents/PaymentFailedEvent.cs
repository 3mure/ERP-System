namespace BuildingBlocks.IntegrationEvents
{
    /// <summary>
    /// Event published when a payment fails.
    /// Consumed by Catalog Service to restore reserved stock.
    /// </summary>
    public record PaymentFailedEvent(
        int OrderId,
        string UserId,
        decimal Amount,
        string? ErrorMessage,
        DateTime FailedAt,
        List<PaymentFailedItemDto> Items,
        int BranchId = 0,
        int? WarehouseId = null
    );

    public record PaymentFailedItemDto(
        int ProductId,
        int Quantity
    );
}
