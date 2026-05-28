using BuildingBlocks.Interfaces;
using Catalog_Service.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace Catalog_Service.Features.SalesAnalyticsFeature.GetSalesTrendAnalysis
{
    public class GetSalesTrendHandler
     : IRequestHandler<GetSalesTrendQuery, List<SalesTrendPointDto>>
    {
        private readonly IBaseRepository<SalesOrder> _salesOrderRepo;

        public GetSalesTrendHandler(IBaseRepository<SalesOrder> salesOrderRepo)
        {
            _salesOrderRepo = salesOrderRepo;
        }

        public async Task<List<SalesTrendPointDto>> Handle(
            GetSalesTrendQuery query,
            CancellationToken cancellationToken)
        {
            var filter = query.TrendFilter.ToLower();
            var (startDate, endDate) = GetDateRange(filter);

            // Fetch only the columns we need — OrderDate + TotalAmount
            var orders = await _salesOrderRepo
                .Get(o => o.Status == "Completed"
                       && !o.IsDeleted
                       && o.OrderDate >= startDate
                       && o.OrderDate <= endDate)
                .Select(o => new { o.OrderDate, o.TotalAmount })
                .ToListAsync(cancellationToken);

            // Group and label based on selected filter
            var result = filter switch
            {
                // Daily → group by hour → "8 AM", "2 PM"
                "daily" => orders
                    .GroupBy(o => o.OrderDate.Hour)
                    .OrderBy(g => g.Key)
                    .Select(g => new SalesTrendPointDto(
                        Label: FormatHour(g.Key),
                        Value: g.Sum(o => o.TotalAmount)))
                    .ToList(),

                // Monthly → group by month → "Jan 2026"
                "monthly" => orders
                    .GroupBy(o => new DateTime(o.OrderDate.Year, o.OrderDate.Month, 1))
                    .OrderBy(g => g.Key)
                    .Select(g => new SalesTrendPointDto(
                        Label: g.Key.ToString("MMM yyyy"),
                        Value: g.Sum(o => o.TotalAmount)))
                    .ToList(),

                // Weekly (default) → group by ISO week → "Week 16"
                _ => orders
                    .GroupBy(o => ISOWeek.GetWeekOfYear(o.OrderDate))
                    .OrderBy(g => g.Key)
                    .Select(g => new SalesTrendPointDto(
                        Label: $"Week {g.Key}",
                        Value: g.Sum(o => o.TotalAmount)))
                    .ToList()
            };

            return result;
        }

        // ── Helpers ──────────────────────────────────────────────

        /// <summary>
        /// Returns the date window for the selected filter.
        ///   daily   → today only (midnight → now)
        ///   weekly  → last 7 days
        ///   monthly → last 12 months
        /// </summary>
        private static (DateTime start, DateTime end) GetDateRange(string filter) =>
            filter switch
            {
                "daily" => (DateTime.UtcNow.Date, DateTime.UtcNow),
                "monthly" => (DateTime.UtcNow.AddMonths(-12), DateTime.UtcNow),
                _ => (DateTime.UtcNow.AddDays(-7), DateTime.UtcNow)
            };

        /// <summary>
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
