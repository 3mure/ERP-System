using Catalog_Service.Features.Customers.GetCustomers;

namespace Catalog_Service.Features.Customers.SalesOrders
{
    public record GetSalesOrdersResult(
    List<SalesOrderRowDto> Data,
    PaginationDto Pagination
    );

}