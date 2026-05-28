using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Catalog_Service.Features.SalesAnalyticsFeature.GetRushHourAnalysis
{
    [ApiController]
    [Route("api/v1/dashboard/sales-analytics-charts")]
    public class dashboard : ControllerBase
    {
        private readonly IMediator _mediator;

        public dashboard(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// GET /api/v1/dashboard/sales-analytics-charts/rush-hour
        ///
        /// Returns how many orders were placed each hour of the day.
        /// Used to plot the Rush Hour Analysis chart on the dashboard.
        /// </summary>
        [HttpGet("rush-hour")]
        [ProducesResponseType(typeof(List<RushHourPointDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetRushHourAnalysis()
        {
            var result = await _mediator.Send(new GetRushHourAnalysisQuery());
            return Ok(result);
        }
    }
}
