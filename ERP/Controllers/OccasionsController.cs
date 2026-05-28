using MediatR;
using Microsoft.AspNetCore.Mvc;
using Catalog_Service.Features.OccasionsFeature;
using Catalog_Service.Features.OccasionsFeature.CreateOccasion;
using Catalog_Service.Features.OccasionsFeature.DeleteOccasion;
using Catalog_Service.Features.OccasionsFeature.GetAllOccasions;
using Catalog_Service.Features.OccasionsFeature.UpdateOccasion;
using Catalog_Service.Features.Shared;

namespace Catalog_Service.Controllers
{
    [ApiController]
    [Route("api")]
    public class OccasionsController : ControllerBase
    {
        private readonly IMediator _mediator;
        public OccasionsController(IMediator mediator) => _mediator = mediator;

        [HttpGet("catalog/occasions")]
        public async Task<IActionResult> GetAllCatalog()
        {
            var result = await _mediator.Send(new GetAllOccasionsQuery());
            return Ok(result);
        }

        [HttpPost("v1/occasions")]
        public async Task<IActionResult> Create([FromBody] CreateOccasionDto dto)
        {
            var result = await _mediator.Send(new CreateOccasionCommand(dto));

            if (!result.IsSuccess)
            {
                return BadRequest(EndpointResponse<RequestResponse<CreateOccasionDto>>.ErrorResponse(
                    result.Message,
                    400));
            }

            return StatusCode(201,
                EndpointResponse<RequestResponse<CreateOccasionDto>>.SuccessResponse(
                    result,
                    "Occasion created successfully",
                    201));
        }

        [HttpPut("occasions/{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateOccasionDto dto)
        {
            var command = new UpdateOccasionCommand(id, dto);
            var result = await _mediator.Send(command);
            if (!result.IsSuccess)
                return NotFound(EndpointResponse<string>.NotFoundResponse($"Occasion with id {id} not found."));

            return Ok(EndpointResponse<int>.SuccessResponse(result.Data!, result.Message));
        }

        [HttpDelete("occasions/{id:int}")]
        public async Task<IActionResult> Delete(int id, CancellationToken ct)
        {
            var result = await _mediator.Send(new DeleteOccasionCommand(id), ct);

            if (!result.IsSuccess)
                return BadRequest(EndpointResponse<bool>.ErrorResponse(result.Message));

            return Ok(EndpointResponse<bool>.SuccessResponse(result.Data, result.Message));
        }

        [HttpGet("occasions/{occasionId:int}/products")]
        public async Task<IActionResult> GetOccasionProducts(int occasionId)
        {
            var result = await _mediator.Send(new GetAllOccasionProductsQuery(occasionId));
            return result.IsSuccess ? Ok(result) : NotFound(result);
        }

        [HttpPut("adoccasions/{id:int}")]
        public async Task<IActionResult> ActivateDeactivate(int id, [FromBody] ActivateDeactivateOcassionDto dto)
        {
            var command = new ActivateDeactivateOcassionComand(id, dto);
            var result = await _mediator.Send(command);
            if (!result.IsSuccess)
                return BadRequest(EndpointResponse<string>.ErrorResponse($"Occasion with id {id} not found."));

            return Ok(EndpointResponse<int>.SuccessResponse(result.Data!, result.Message));
        }
    }
}
