using Dashboard_Service.Features.OverviewKpis;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Catalog_Service.Features.OverviewKpis.GetNetProfit
{
    [ApiController]
    [Route("api/v1/dashboard/overview-kpis")]
    public class dashboard : ControllerBase
    {
        private readonly IMediator _mediator;
        public dashboard(IMediator mediator) => _mediator = mediator;

        /// <summary>
        /// GET /api/v1/dashboard/overview-kpis/net-profit?date_range=last_30_days
        /// date_range: last_7_days | last_30_days | last_90_days | last_year
        /// </summary>
        [HttpGet("net-profit")]
        [ProducesResponseType(typeof(KpiResultDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetNetProfit(
            [FromQuery(Name = "date_range")] string dateRange = "last_30_days")
        {
            if (!DateRangeHelper.IsValid(dateRange))
                return BadRequest(DateRangeHelper.InvalidMessage(dateRange));

            var result = await _mediator.Send(new GetNetProfitQuery(dateRange));
            return Ok(result);
        }
    }

}
