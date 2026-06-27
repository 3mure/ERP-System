using Catalog_Service.Features.ProductsFeature.GetInventorySummary;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Catalog_Service.Features.Inventory.GetInventorySummary
{
    [ApiController]
    [Route("api/v1/inventory")]
    public class InventorySummaryController : ControllerBase
    {
        private readonly IMediator _mediator;
        public InventorySummaryController(IMediator mediator) => _mediator = mediator;

        /// <summary>
        /// GET /api/v1/inventory/summary?warehouse_id=all
        /// warehouse_id: "all" | "WH-001" | "WH-002" ...
        /// Recalculates all 5 boxes when a warehouse is selected from dropdown.
        /// </summary>
        [HttpGet("summary")]
        [ProducesResponseType(typeof(InventorySummaryDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetInventorySummary(
            [FromQuery(Name = "warehouse_id")] string warehouseId = "all")
        {
            var result = await _mediator.Send(new GetInventorySummaryQuery(warehouseId));
            return Ok(result);
        }
    }

}
