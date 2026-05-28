using MediatR;

namespace Catalog_Service.Features.Customers.GetCustomers
{
    public record GetCustomersQuery(string? Search, int Page)
     : IRequest<GetCustomersResult>;
}
