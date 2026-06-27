using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Catalog_Service.Features.ProductPerformanceAndGetLowMovementStock.GetLowMovementStock
{
    [ApiController]
    [Route("api/v1/dashboard/product-performance")]
    public class productperformance : ControllerBase
    {
        private readonly IMediator _mediator;

        public productperformance(IMediator mediator)
            => _mediator = mediator;

        /// <summary>
        /// GET /api/v1/dashboard/product-performance/low-movement
        ///
        /// Returns the top 5 products that have stock on hand
        /// but haven't appeared on a completed invoice in the longest time.
        ///
        /// DaysWithoutSale = days since last completed sale.
        /// If product was never sold, counted from its creation date.
        /// TotalStock = sum of current_stock across all warehouses.
        /// </summary>
        [HttpGet("low-movement")]
        [ProducesResponseType(typeof(List<LowMovementStockDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetLowMovementStock()
        {
            var result = await _mediator.Send(new GetLowMovementStockQuery());
            return Ok(result);
        }
    }

}
