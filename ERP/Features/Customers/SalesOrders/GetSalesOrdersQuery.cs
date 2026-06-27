using MediatR;

namespace Catalog_Service.Features.Customers.SalesOrders
{
    public record GetSalesOrdersQuery(
    string? TimeFilter,
    string? StatusFilter,
    string? Search,
    int Page
    ) : IRequest<GetSalesOrdersResult>;
}
