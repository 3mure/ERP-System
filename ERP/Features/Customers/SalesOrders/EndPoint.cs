using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Catalog_Service.Features.Customers.SalesOrders
{
    [ApiController]
    [Route("api/v1/sales-orders")]
    public class salesorders : ControllerBase
    {
        private readonly IMediator _mediator;
        public salesorders(IMediator mediator) => _mediator = mediator;

        /// <summary>
        /// GET /api/v1/sales-orders?time_filter=all_time&amp;status_filter=all&amp;search=&amp;page=1
        ///
        /// time_filter:   today | this_week | this_month | all_time
        /// status_filter: completed | returned | all
        /// search:        matches order_id, invoice_no, or customer_name
        ///
        /// StatusColor in response: "green" for Completed, "red" for Returned.
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(GetSalesOrdersResult), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetSalesOrders(
            [FromQuery(Name = "time_filter")] string? timeFilter = "all_time",
            [FromQuery(Name = "status_filter")] string? statusFilter = "all",
            [FromQuery] string? search = null,
            [FromQuery] int page = 1)
        {
            var result = await _mediator.Send(
                new GetSalesOrdersQuery(timeFilter, statusFilter, search, page));

            return Ok(result);
        }
    }
}
