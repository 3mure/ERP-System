using BuildingBlocks.Interfaces;
using Catalog_Service.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Catalog_Service.Features.Customers.GetCustomers
{
    public class GetCustomersHandler
    : IRequestHandler<GetCustomersQuery, GetCustomersResult>
    {
        private readonly IBaseRepository<Customer> _customersRepo;
        private readonly IBaseRepository<SalesOrder> _ordersRepo;

        private const int PageSize = 10;

        // Loyalty thresholds for color badge
        private const int LoyaltyGoodThreshold = 100;  // green
        private const int LoyaltyAverageThreshold = 30;   // yellow — below = red

        public GetCustomersHandler(
            IBaseRepository<Customer> customersRepo,
            IBaseRepository<SalesOrder> ordersRepo)
        {
            _customersRepo = customersRepo;
            _ordersRepo = ordersRepo;
        }

        public async Task<GetCustomersResult> Handle(
            GetCustomersQuery query,
            CancellationToken cancellationToken)
        {
            // ── Step 1: Filter customers by search term ───────────
            var customersQuery = _customersRepo
                .Get(c => !c.IsDeleted && c.IsActive);

            if (!string.IsNullOrWhiteSpace(query.Search))
            {
                var search = query.Search.ToLower();
                customersQuery = customersQuery.Where(c =>
                    c.NameEn.ToLower().Contains(search) ||
                    c.Phone.Contains(search));
            }

            // ── Step 2: Paginate ──────────────────────────────────
            var totalCount = await customersQuery.CountAsync(cancellationToken);
            var totalPages = (int)Math.Ceiling((double)totalCount / PageSize);
            var page = Math.Max(1, query.Page);

            var customers = await customersQuery
                .OrderBy(c => c.NameEn)
                .Skip((page - 1) * PageSize)
                .Take(PageSize)
                .Select(c => new
                {
                    c.Id,
                    c.NameEn,
                    c.Phone,
                    c.LoyaltyPoints,
                    RegisteredByName = c.RegisteredByUser != null
                        ? c.RegisteredByUser.Name
                        : "N/A"
                })
                .ToListAsync(cancellationToken);

            if (!customers.Any())
                return new GetCustomersResult(
                    new List<CustomerRowDto>(),
                    new PaginationDto(page, totalPages));

            var customerIds = customers.Select(c => c.Id).ToList();

            // ── Step 3: Load all orders for these customers in one query
            var orders = await _ordersRepo
                .Get(o => customerIds.Contains((int)o.CustomerId!)
                       && !o.IsDeleted
                       && (o.Status == "Completed" || o.Status == "Returned"))
                .Select(o => new
                {
                    o.CustomerId,
                    o.Status,
                    o.TotalAmount,
                    o.OrderDate
                })
                .ToListAsync(cancellationToken);

            // ── Step 4: Group order stats per customer ────────────
            var ordersByCustomer = orders
                .GroupBy(o => o.CustomerId)
                .ToDictionary(g => g.Key, g => g.ToList());

            // ── Step 5: Build final rows ──────────────────────────
            var rows = customers.Select(c =>
            {
                var customerOrders = ordersByCustomer.TryGetValue(c.Id, out var o)
                    ? o : new();

                var completedOrders = customerOrders
                    .Where(o => o.Status == "Completed")
                    .ToList();

                var totalPurchases = completedOrders.Sum(o => o.TotalAmount);
                var visits = completedOrders.Count;
                var lastVisit = completedOrders.Any()
                    ? completedOrders.Max(o => o.OrderDate)
                    : (DateTime?)null;
                var returns = customerOrders.Count(o => o.Status == "Returned");

                return new CustomerRowDto(
                    CustomerId: c.Id,
                    CustomerName: c.NameEn,
                    Phone: c.Phone,
                    TotalPurchases: FormatEGP(totalPurchases),
                    Visits: visits,
                    LastVisit: lastVisit.HasValue
                                        ? lastVisit.Value.ToString("yyyy-MM-dd")
                                        : "Never",
                    AssignedEmployee: c.RegisteredByName,
                    LoyaltyPoints: c.LoyaltyPoints,
                    LoyaltyStatus: GetLoyaltyStatus(c.LoyaltyPoints),
                    Returns: returns
                );
            }).ToList();

            return new GetCustomersResult(
                Data: rows,
                Pagination: new PaginationDto(page, totalPages));
        }

        // ── Helpers ──────────────────────────────────────────────

        /// <summary>
        /// Color badge logic for the Loyalty column.
        ///   >= 100 → "good"    (green)
        ///   >= 30  → "average" (yellow)
        ///   below  → "low"     (red)
        /// </summary>
        private static string GetLoyaltyStatus(int points) =>
            points >= LoyaltyGoodThreshold ? "good" :
            points >= LoyaltyAverageThreshold ? "average" :
                                                "low";

        private static string FormatEGP(decimal value) =>
            $"EGP {value:N0}";
    }

}
