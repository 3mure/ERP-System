using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Catalog_Service.Features.ProductPerformanceAndGetLowMovementStock.GetTopSellingProducts
{
    [ApiController]
    [Route("api/v1/dashboard/product-performance")]
    public class productperformance : ControllerBase
    {
        private readonly IMediator _mediator;

        public productperformance(IMediator mediator)
            => _mediator = mediator;

        /// <summary>
        /// GET /api/v1/dashboard/product-performance/top-selling
        ///
        /// Returns the top 5 products by units sold.
        /// Percentage = that product's units ÷ ALL units sold across all products.
        /// </summary>
        [HttpGet("top-selling")]
        [ProducesResponseType(typeof(List<TopSellingProductDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetTopSellingProducts()
        {
            var result = await _mediator.Send(new GetTopSellingProductsQuery());
            return Ok(result);
        }
    }
}
