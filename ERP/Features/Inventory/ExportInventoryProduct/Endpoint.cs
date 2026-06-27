using Catalog_Service.Features.Inventory.ExportInventoryProduct;
using MediatR;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/v1/inventory/products/export")]
public class InventorySummary : ControllerBase
{
    private readonly IMediator _mediator;
    public InventorySummary(IMediator mediator) => _mediator = mediator;

    [HttpGet]
    [ProducesResponseType(typeof(FileResult), StatusCodes.Status200OK)]
    public async Task<IActionResult> Export(
        [FromQuery] string? search = null,
        [FromQuery(Name = "warehouse_id")] string warehouseId = "all",
        [FromQuery] string status = "all")
    {
        var csvBytes = await _mediator.Send(new ExportInventoryProductsQuery(search, warehouseId, status));
        var fileName = $"inventory_{DateTime.UtcNow:yyyyMMdd_HHmm}.csv";

        return File(csvBytes, "text/csv", fileName);
    }
}