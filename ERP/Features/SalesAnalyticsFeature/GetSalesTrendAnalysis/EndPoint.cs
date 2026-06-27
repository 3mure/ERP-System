using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Catalog_Service.Features.SalesAnalyticsFeature.GetSalesTrendAnalysis
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
        /// GET /api/v1/dashboard/sales-analytics-charts/sales-trend?trend_filter=weekly
        ///
        /// trend_filter: daily | weekly | monthly
        /// Returns grouped sales totals so the frontend can plot the line chart.
        /// </summary>
        [HttpGet("sales-trend")]
        [ProducesResponseType(typeof(List<SalesTrendPointDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetSalesTrend(
            [FromQuery(Name = "trend_filter")] string trendFilter = "weekly")
        {
            var allowed = new[] { "daily", "weekly", "monthly" };

            if (!allowed.Contains(trendFilter.ToLower()))
                return BadRequest(new
                {
                    error = "Invalid trend_filter value.",
                    allowed = allowed,
                    received = trendFilter
                });

            var result = await _mediator.Send(new GetSalesTrendQuery(trendFilter));
            return Ok(result);
        }
    }

}
