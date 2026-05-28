using MediatR;
using Microsoft.AspNetCore.Mvc;
using Catalog_Service.Features.CategoriesFeature.CreateCategory;
using Catalog_Service.Features.CategoriesFeature.DeleteCategory;
using Catalog_Service.Features.CategoriesFeature.GetActiveCategoryFeature;
using Catalog_Service.Features.CategoriesFeature.GetAllCategories;
using Catalog_Service.Features.CategoriesFeature.UpdateCategory;
using Catalog_Service.Features.CategoriesFeature.UpdateCategoryStatus;
using Catalog_Service.Features.CategoryWithProduct.ViewCategoryProducts;
using Catalog_Service.Features.Shared;

namespace Catalog_Service.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoriesController : ControllerBase
    {
        private readonly IMediator _mediator;
        public CategoriesController(IMediator mediator) => _mediator = mediator;

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateCategoryCommand command)
        {
            var id = await _mediator.Send(command);
            return Created($"/api/categories/{id}", new { Id = id });
        }

        [HttpGet("~/api/v1/categories")]
        public async Task<IActionResult> GetAllV1()
        {
            var result = await _mediator.Send(new GetAllCategoriesQuery());
            if (!result.IsSuccess)
                return BadRequest(EndpointResponse<RequestResponse<List<CategoryViewModel>>>.ErrorResponse(result.Message));
            return Ok(EndpointResponse<RequestResponse<List<CategoryViewModel>>>.SuccessResponse(result));
        }

        [HttpGet("~/api/categories/active")]
        public async Task<IActionResult> GetActive()
        {
            var result = await _mediator.Send(new GetAllActiveCategoriesQuery());
            if (!result.IsSuccess)
                return NotFound(result.Message);
            return Ok(EndpointResponse<List<CategoryactiveViewModel>>.SuccessResponse(result.Data!, result.Message));
        }

        [HttpPut("~/api/v1/categories/{id:int}")]
        public async Task<IActionResult> UpdateV1(int id, [FromBody] UpdateCategoryDto dto)
        {
            var result = await _mediator.Send(new UpdateCategoryCommand(id, dto));
            if (!result.IsSuccess)
                return BadRequest(EndpointResponse<RequestResponse<bool>>.ErrorResponse(result.Message, statusCode: 400));
            return Ok(EndpointResponse<RequestResponse<bool>>.SuccessResponse(result, "Category updated successfully", statusCode: 200));
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id, CancellationToken ct)
        {
            var result = await _mediator.Send(new DeleteCategoryCommand(id), ct);
            if (!result.IsSuccess)
                return BadRequest(EndpointResponse<bool>.ErrorResponse(result.Message));
            return Ok(EndpointResponse<bool>.SuccessResponse(result.Data, result.Message));
        }

        [HttpPatch("~/api/v1/categories/{id:int}/activate")]
        public async Task<IActionResult> Activate(int id)
        {
            var result = await _mediator.Send(new ActivateCategoryCommand(id));
            if (!result.IsSuccess)
                return BadRequest(EndpointResponse<RequestResponse<bool>>.ErrorResponse(result.Message, 400));
            return Ok(EndpointResponse<RequestResponse<bool>>.SuccessResponse(result, "Category activated successfully", 200));
        }

        [HttpPatch("~/api/v1/categories/{id:int}/deactivate")]
        public async Task<IActionResult> Deactivate(int id)
        {
            var result = await _mediator.Send(new DeactivateCategoryCommand(id));
            if (!result.IsSuccess)
                return BadRequest(EndpointResponse<RequestResponse<bool>>.ErrorResponse(result.Message, 400));
            return Ok(EndpointResponse<RequestResponse<bool>>.SuccessResponse(result, "Category deactivated successfully", 200));
        }

        [HttpGet("~/api/v1/categories/{categoryId:int}/products")]
        public async Task<IActionResult> GetCategoryProducts(int categoryId)
        {
            var result = await _mediator.Send(new ViewCategoryProductsQuery(categoryId));
            return Ok(result);
        }
    }
}
