using BuildingBlocks.Interfaces;
using Catalog_Service.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Catalog_Service.Features.SalesAnalyticsFeature.GetRushHourAnalysis
{
    public class GetRushHourAnalysisHandler
    : IRequestHandler<GetRushHourAnalysisQuery, List<RushHourPointDto>>
    {
        private readonly IBaseRepository<SalesOrder> _salesOrderRepo;

        public GetRushHourAnalysisHandler(IBaseRepository<SalesOrder> salesOrderRepo)
        {
            _salesOrderRepo = salesOrderRepo;
        }

        public async Task<List<RushHourPointDto>> Handle(
            GetRushHourAnalysisQuery query,
            CancellationToken cancellationToken)
        {
            // Pull only completed orders — no need to load full entity,
            // we only care about the OrderDate column
            var orders = await _salesOrderRepo
                .Get(o => o.Status == "Completed" && !o.IsDeleted)
                .Select(o => new { o.OrderDate })
                .ToListAsync(cancellationToken);

            // Group by hour of day → count how many orders per hour
            var result = orders
                .GroupBy(o => o.OrderDate.Hour)
                .OrderBy(g => g.Key)
                .Select(g => new RushHourPointDto(
                    Time: FormatHour(g.Key),
                    Volume: g.Count()))
                .ToList();

            return result;
        }

        // ── Helper ───────────────────────────────────────────────
        /// <summary>
        /// Converts 24h int → readable AM/PM label.
        /// 0 → "12 AM" | 8 → "8 AM" | 12 → "12 PM" | 14 → "2 PM"
        /// </summary>
        private static string FormatHour(int hour) =>
            hour switch
            {
                0 => "12 AM",
                < 12 => $"{hour} AM",
                12 => "12 PM",
                _ => $"{hour - 12} PM"
            };
    }

}
