using BuildingBlocks.Interfaces;
using Catalog_Service.Entities;
using Catalog_Service.Features.Customers.GetCustomers;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Catalog_Service.Features.Customers.SalesOrders
{
    public class GetSalesOrdersHandler
      : IRequestHandler<GetSalesOrdersQuery, GetSalesOrdersResult>
    {
        private readonly IBaseRepository<SalesOrder> _ordersRepo;
        private const int PageSize = 10;

        public GetSalesOrdersHandler(IBaseRepository<SalesOrder> ordersRepo)
            => _ordersRepo = ordersRepo;

        public async Task<GetSalesOrdersResult> Handle(
            GetSalesOrdersQuery query,
            CancellationToken cancellationToken)
        {
            // ── Step 1: Base query — exclude soft-deleted ─────────
            var ordersQuery = _ordersRepo.Get(o => !o.IsDeleted);

            // ── Step 2: Time filter ───────────────────────────────
            ordersQuery = ApplyTimeFilter(ordersQuery, query.TimeFilter);

            // ── Step 3: Status filter ─────────────────────────────
            if (!string.IsNullOrWhiteSpace(query.StatusFilter)
                && query.StatusFilter.ToLower() != "all")
            {
                // Capitalize first letter to match stored values ("Completed", "Returned")
                var status = char.ToUpper(query.StatusFilter[0]) + query.StatusFilter[1..].ToLower();
                ordersQuery = ordersQuery.Where(o => o.Status == status);
            }

            // ── Step 4: Search (order_id | invoice_no | customer_name) ──
            if (!string.IsNullOrWhiteSpace(query.Search))
            {
                var search = query.Search.ToLower();
                ordersQuery = ordersQuery.Where(o =>
                    o.OrderCode.ToLower().Contains(search) ||
                    o.InvoiceNo.ToLower().Contains(search) ||
                    (o.Customer != null &&
                     o.Customer.NameEn.ToLower().Contains(search)));
            }

            // ── Step 5: Paginate ──────────────────────────────────
            var totalCount = await ordersQuery.CountAsync(cancellationToken);
            var totalPages = (int)Math.Ceiling((double)totalCount / PageSize);
            var page = Math.Max(1, query.Page);

            var orders = await ordersQuery
                .OrderByDescending(o => o.OrderDate)
                .Skip((page - 1) * PageSize)
                .Take(PageSize)
                .Select(o => new
                {
                    o.OrderCode,
                    o.InvoiceNo,
                    o.OrderDate,
                    o.TotalAmount,
                    o.Status,
                    CustomerName = o.Customer != null ? o.Customer.NameEn : "Walk-in",
                    CashierName = o.Cashier != null ? o.Cashier.Name : "N/A"
                })
                .ToListAsync(cancellationToken);

            // ── Step 6: Map to DTOs ───────────────────────────────
            var rows = orders.Select(o => new SalesOrderRowDto(
                OrderId: o.OrderCode,
                InvoiceNo: o.InvoiceNo,
                Date: o.OrderDate.ToString("yyyy-MM-dd"),
                CustomerName: o.CustomerName,
                Amount: $"EGP {o.TotalAmount:N0}",
                Cashier: o.CashierName,
                Status: o.Status,
                StatusColor: o.Status == "Completed" ? "green" : "red"
            )).ToList();

            return new GetSalesOrdersResult(
                Data: rows,
                Pagination: new PaginationDto(page, totalPages));
        }

        // ─────────────────────────────────────────────────────────
        // TIME FILTER
        // Applies the correct date window based on the dropdown value.
        //   today      → orders placed today only
        //   this_week  → last 7 days
        //   this_month → last 30 days
        //   all_time   → no date filter (default)
        // ─────────────────────────────────────────────────────────
        private static IQueryable<SalesOrder> ApplyTimeFilter(
            IQueryable<SalesOrder> query, string? filter)
        {
            var now = DateTime.UtcNow;

            return filter?.ToLower() switch
            {
                "today" => query.Where(o => o.OrderDate >= now.Date),
                "this_week" => query.Where(o => o.OrderDate >= now.AddDays(-7)),
                "this_month" => query.Where(o => o.OrderDate >= now.AddDays(-30)),
                _ => query   // "all_time" or null → no date filter
            };
        }
    }
}
