using MediatR;

namespace Catalog_Service.Features.Customers.CreateCustomer
{
    public record CreateCustomerCommand(CreateCustomerRequest Request)
    : IRequest<CreateCustomerResult>;
}
