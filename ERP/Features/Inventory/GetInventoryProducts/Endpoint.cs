using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Catalog_Service.Features.Inventory.GetInventoryProducts
{
    [ApiController]
    [Route("api/v1/inventory/GetInventoryProducts")]
    public class InventorySummary : ControllerBase
    {
        private readonly IMediator _mediator;
        public InventorySummary(IMediator mediator) => _mediator = mediator;

        /// <summary>
        /// GET /api/v1/inventory/products?search=&amp;warehouse_id=all&amp;status=all&amp;page=1
        /// status: all | in_stock | low_stock | out_of_stock
        /// </summary>
        [HttpGet("products")]
        [ProducesResponseType(typeof(GetInventoryProductsResult), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetInventoryProducts(
            [FromQuery] string? search = null,
            [FromQuery(Name = "warehouse_id")] string warehouseId = "all",
            [FromQuery] string status = "all",
            [FromQuery] int page = 1)
        {
            var result = await _mediator.Send(
                new GetInventoryProductsQuery(search, warehouseId, status, page));

            return Ok(result);
        }
    }
}
