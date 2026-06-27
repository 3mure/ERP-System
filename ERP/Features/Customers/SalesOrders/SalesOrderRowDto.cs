namespace Catalog_Service.Features.Customers.SalesOrders
{
    public record SalesOrderRowDto(
     string OrderId,        // "ORD-001"
     string InvoiceNo,      // "INV-2024-001"
     string Date,           // "2026-01-28"
     string CustomerName,
     string Amount,         // "EGP 45,890"
     string Cashier,
     string Status,         // "Completed" | "Returned"
     string StatusColor     // "green" | "red" — for the badge
 );
}