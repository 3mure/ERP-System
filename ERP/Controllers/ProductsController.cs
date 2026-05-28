using MediatR;
using Microsoft.AspNetCore.Mvc;
using Catalog_Service.Features.ProductsFeature.CreateProduct;
using Catalog_Service.Features.ProductsFeature.DeleteProduct;
using Catalog_Service.Features.ProductsFeature.GetBestSellers;
using Catalog_Service.Features.ProductsFeature.GetProductDetails;
using Catalog_Service.Features.ProductsFeature.ProductExist;
using Catalog_Service.Features.ProductsFeature.Search;
using Catalog_Service.Features.ProductsFeature.StockManagement;
using Catalog_Service.Features.ProductsFeature.UpdateProduct;
using Catalog_Service.Features.ProductsFeature.UpdateProductStatus;
using Catalog_Service.Features.RateDeliveredItem;
using Catalog_Service.Features.Shared;

namespace Catalog_Service.Controllers
{
    [ApiController]
    [Route("api/products")]
    public class ProductsController : ControllerBase
    {
        private readonly IMediator _mediator;
        public ProductsController(IMediator mediator) => _mediator = mediator;

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateProductCommand command)
        {
            var result = await _mediator.Send(command);
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("best-sellers")]
        public async Task<IActionResult> GetBestSellers([FromQuery] int? count)
        {
            try
            {
                var query = new GetBestSellersQuery(count ?? 10);
                var result = await _mediator.Send(query);

                if (!result.IsSuccess)
                    return BadRequest(EndpointResponse<object>.ErrorResponse(result.Message));

                return Ok(EndpointResponse<object>.SuccessResponse(
                    result.Data!,
                    "Best sellers retrieved successfully"));
            }
            catch (Exception ex)
            {
                return Problem(title: "Server Error", detail: ex.Message, statusCode: 500);
            }
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var result = await _mediator.Send(new GetProductDetailsQuery(id));

                if (!result.IsSuccess)
                    return NotFound(EndpointResponse<object>.NotFoundResponse(result.Message));

                return Ok(EndpointResponse<object>.SuccessResponse(
                    result.Data!,
                    "Product details retrieved successfully"));
            }
            catch (Exception ex)
            {
                return Problem(title: "Server Error", detail: ex.Message, statusCode: 500);
            }
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateProductCommand command)
        {
            command.Id = id;
            var result = await _mediator.Send(command);
            return StatusCode(result.StatusCode, result);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _mediator.Send(new DeleteProductCommand(id));
            return StatusCode(result.StatusCode, result);
        }

        [HttpPatch("{id:int}/status")]
        public async Task<IActionResult> UpdateStatus(int id, [FromBody] bool isAvailable)
        {
            var result = await _mediator.Send(new UpdateProductStatusCommand(id, isAvailable));
            return StatusCode(result.StatusCode, result);
        }

        /// <summary>Search products (GET + query filters; repeat CategoryIds/OccasionIds for multiple values).</summary>
        [HttpGet("~/api/v1/products/search")]
        public async Task<IActionResult> Search([FromQuery] DTOs? filters)
        {
            var result = await _mediator.Send(new SearchProductsQuery(filters ?? new DTOs()));

            if (!result.IsSuccess)
                return BadRequest(EndpointResponse<RequestResponse<List<ViewModel>>>.ErrorResponse(result.Message));

            return Ok(EndpointResponse<RequestResponse<List<ViewModel>>>.SuccessResponse(result));
        }

        [HttpGet("~/api/v1/products/exists")]
        public async Task<IActionResult> Exists([FromQuery] ProductExistsDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto?.Name))
                return BadRequest(EndpointResponse<RequestResponse<bool>>.ErrorResponse("Name query parameter is required.", 400));

            var result = await _mediator.Send(new ProductExistsQuery(dto));
            return Ok(EndpointResponse<RequestResponse<bool>>.SuccessResponse(result));
        }

        [HttpPost("~/api/v1/products/{productId:int}/reviews")]
        public async Task<IActionResult> AddReview(int productId, [FromBody] AddReviewDto dto)
        {
            var userId = HttpContext.User.Identity?.Name ?? "test-user";
            const string userName = "Test User";

            var result = await _mediator.Send(
                new AddReviewCommand(productId, userId, userName, dto.Rating, dto.Comment));

            if (!result.IsSuccess)
            {
                return BadRequest(EndpointResponse<RequestResponse<bool>>.ErrorResponse(result.Message, 400));
            }

            return Ok(EndpointResponse<RequestResponse<bool>>.SuccessResponse(result, "Review added successfully"));
        }

        [HttpPut("{productId:int}/stock/settings")]
        public async Task<IActionResult> UpdateStockSettings(int productId, [FromBody] UpdateStockSettingsCommand command)
        {
            if (productId != command.ProductId)
                return BadRequest("Route product id and body ProductId must match.");
            var result = await _mediator.Send(command);
            return StatusCode(result.StatusCode, result);
        }
    }
}
