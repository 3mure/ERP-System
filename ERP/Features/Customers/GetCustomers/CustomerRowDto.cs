namespace Catalog_Service.Features.Customers.GetCustomers
{
    public record CustomerRowDto(
    int CustomerId,
    string CustomerName,
    string Phone,
    string TotalPurchases,      // formatted: "EGP 1,000,890"
    int Visits,              // count of completed orders
    string LastVisit,           // formatted date: "2025-01-28"
    string AssignedEmployee,
    int LoyaltyPoints,
    string LoyaltyStatus,       // "good" | "average" | "low" — for color badge
    int Returns              // count of returned orders
    );

}