using MediatR;
using Microsoft.AspNetCore.Mvc;
using Catalog_Service.Features.ProductsFeature.GetInventorySummary;
using Catalog_Service.Features.ProductsFeature.StockManagement;

namespace Catalog_Service.Controllers
{
    /// <summary>Catalog inventory: summaries and per-product stock changes.</summary>
    [ApiController]
    [Route("api/inventory")]
    public class InventoryController : ControllerBase
    {
        private readonly IMediator _mediator;
        public InventoryController(IMediator mediator) => _mediator = mediator;

        [HttpGet]
        public async Task<IActionResult> GetSummary([FromQuery] string? status)
        {
            var result = await _mediator.Send(new GetInventorySummaryQuery(status));
            return Ok(result);
        }

        [HttpPost("products/{productId:int}/stock/add")]
        public async Task<IActionResult> AddStock(int productId, [FromBody] int quantity)
        {
            var result = await _mediator.Send(new AddStockCommand(productId, quantity));
            return StatusCode(result.StatusCode, result);
        }

        /// <summary>Sets absolute on-hand quantity for the product (idempotent replace).</summary>
        [HttpPut("products/{productId:int}/stock")]
        public async Task<IActionResult> SetStock(int productId, [FromBody] int quantity)
        {
            var result = await _mediator.Send(new SetStockCommand(productId, quantity));
            return StatusCode(result.StatusCode, result);
        }
    }
}
