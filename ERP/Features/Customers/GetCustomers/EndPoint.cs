using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Catalog_Service.Features.Customers.GetCustomers
{
    [ApiController]
    [Route("api/v1/customers")]
    public class customers : ControllerBase
    {
        private readonly IMediator _mediator;
        public customers(IMediator mediator) => _mediator = mediator;

        /// <summary>
        /// GET /api/v1/customers?search=Ahmed&amp;page=1
        ///
        /// Returns paginated customer list with calculated fields:
        /// total_purchases, visits, last_visit, returns, loyalty_status.
        /// loyalty_status drives the color badge: good=green, average=yellow, low=red.
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(GetCustomersResult), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetCustomers(
            [FromQuery] string? search = null,
            [FromQuery] int page = 1)
        {
            var result = await _mediator.Send(new GetCustomersQuery(search, page));
            return Ok(result);
        }
    }

}
