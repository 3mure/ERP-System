// ============================================================
// SHARED HELPERS — used by all 6 KPI slices
// Put this file in: Dashboard_Service/Features/OverviewKpis/Helpers.cs
// ============================================================

namespace Dashboard_Service.Features.OverviewKpis;

// ────────────────────────────────────────────────────────────
// DATE RANGE HELPER
// Converts the ?date_range= string into two DateTime windows:
//   - current period  (what the user is viewing)
//   - previous period (for trend comparison)
//
// Example for "last_30_days" called on April 18:
//   current  → Mar 19 → Apr 18
//   previous → Feb 17 → Mar 18
// ────────────────────────────────────────────────────────────
public static class DateRangeHelper
{
    private static readonly string[] _allowed =
        { "last_7_days", "last_30_days", "last_90_days", "last_year" };

    public static bool IsValid(string dateRange) =>
        _allowed.Contains(dateRange.ToLower());

    public static object InvalidMessage(string received) => new
    {
        error = "Invalid date_range value.",
        allowed = _allowed,
        received = received
    };

    /// <summary>
    /// Returns (currentStart, currentEnd, previousStart, previousEnd).
    /// previousEnd = currentStart (the day before current period starts)
    /// previousStart = previousEnd minus the same number of days
    /// </summary>
    public static (DateTime currentStart, DateTime currentEnd,
                   DateTime prevStart, DateTime prevEnd)
        Resolve(string dateRange)
    {
        var now = DateTime.UtcNow;
        var days = dateRange.ToLower() switch
        {
            "last_7_days" => 7,
            "last_90_days" => 90,
            "last_year" => 365,
            _ => 30   // default: last_30_days
        };

        var currentStart = now.AddDays(-days);
        var currentEnd = now;
        var prevEnd = currentStart;
        var prevStart = prevEnd.AddDays(-days);

        return (currentStart, currentEnd, prevStart, prevEnd);
    }
}


// ────────────────────────────────────────────────────────────
// TREND HELPER
// Compares current vs previous value and returns:
//   - Formatted trend string  e.g. "+12.5%"
//   - Status string           "positive" | "negative" | "neutral"
//
// Two flavors:
//   Calc()         → higher is BETTER  (profit, sales, AOV, satisfaction)
//   CalcInverted() → lower  is BETTER  (return rate)
// ────────────────────────────────────────────────────────────
public static class TrendHelper
{
    /// <summary>
    /// Standard: value going UP = positive (green).
    /// Used for: Net Profit, Total Sales, AOV, Stock Turnover, Customer Satisfaction.
    /// </summary>
    public static (string trendText, string status) Calc(decimal current, decimal previous)
    {
        if (previous == 0)
            return ("N/A", "neutral");

        var change = ((current - previous) / previous) * 100;
        var sign = change >= 0 ? "+" : "";
        var status = change > 0 ? "positive" : change < 0 ? "negative" : "neutral";

        return ($"{sign}{change:F1}%", status);
    }

    /// <summary>
    /// Inverted: value going UP = negative (red).
    /// Used for: Return Rate (a higher return rate is bad).
    /// </summary>
    public static (string trendText, string status) CalcInverted(
        decimal current, decimal previous)
    {
        if (previous == 0)
            return ("N/A", "neutral");

        var change = ((current - previous) / previous) * 100;
        var sign = change >= 0 ? "+" : "";

        // Flip: going up is BAD → negative; going down is GOOD → positive
        var status = change > 0 ? "negative" : change < 0 ? "positive" : "neutral";

        return ($"{sign}{change:F1}%", status);
    }
}


// ────────────────────────────────────────────────────────────
// FORMAT HELPER
// Formats decimal numbers into the display strings shown in the UI.
//
// Examples:
//   192100  → "EG192.1k"
//   15100   → "EG15.1k"
//   850     → "EG850"
// ────────────────────────────────────────────────────────────
public static class FormatHelper
{
    /// <summary>
    /// Formats a money value with "EG" prefix and "k" suffix if >= 1000.
    /// </summary>
    public static string FormatEGP(decimal value)
    {
        if (value >= 1_000_000)
            return $"EG{value / 1_000_000:F1}M";

        if (value >= 1_000)
            return $"EG{value / 1_000:F1}k";

        return $"EG{value:F0}";
    }
}
