namespace BuildingBlocks.IntegrationEvents
{
    public record OrderDeliveredEvent(
        int OrderId,
        string UserId,
        decimal OrderTotal,
        DateTime DeliveredAt,
        List<OrderDeliveredItemDto> Items,
        int BranchId = 0,
        int? WarehouseId = null
    );

    public record OrderDeliveredItemDto(
        int ProductId,
        int Quantity
    );
}
