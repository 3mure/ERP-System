using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Catalog_Service.Features.Customers.CreateCustomer
{
    [ApiController]
    [Route("api/v1/customers")]
    public class customers : ControllerBase
    {
        private readonly IMediator _mediator;
        public customers(IMediator mediator) => _mediator = mediator;

        /// <summary>
        /// POST /api/v1/customers
        /// Creates a new customer profile.
        /// Required: basic_information.name_en and basic_information.phone_number
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(CreateCustomerResult), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> CreateCustomer(
            [FromBody] CreateCustomerRequest request)
        {
            // Model validation handles [Required] attributes automatically
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _mediator.Send(new CreateCustomerCommand(request));

            if (!result.Success)
                // 409 Conflict = phone already exists
                return Conflict(new { message = result.Message });

            return StatusCode(StatusCodes.Status201Created, new
            {
                message = result.Message,
                customerId = result.CustomerId
            });
        }
    }

}
