using Catalog_Service.Features.OverviewKpis.GetNetProfit;
using Dashboard_Service.Features.OverviewKpis;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Catalog_Service.Features.OverviewKpis.GetStockTurnoverRate
{
    [ApiController]
    [Route("api/v1/dashboard/overview-kpis")]
    public class dashboard : ControllerBase
    {
        private readonly IMediator _mediator;
        public dashboard(IMediator mediator) => _mediator = mediator;

        /// <summary>
        /// GET /api/v1/dashboard/overview-kpis/stock-turnover-rate?date_range=last_30_days
        /// </summary>
        [HttpGet("stock-turnover-rate")]
        [ProducesResponseType(typeof(KpiResultDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetStockTurnoverRate(
            [FromQuery(Name = "date_range")] string dateRange = "last_30_days")
        {
            if (!DateRangeHelper.IsValid(dateRange))
                return BadRequest(DateRangeHelper.InvalidMessage(dateRange));

            var result = await _mediator.Send(new GetStockTurnoverRateQuery(dateRange));
            return Ok(result);
        }
    }
}
