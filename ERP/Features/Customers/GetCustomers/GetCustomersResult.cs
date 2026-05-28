namespace Catalog_Service.Features.Customers.GetCustomers
{
    public record GetCustomersResult(
        List<CustomerRowDto> Data,
        PaginationDto Pagination
    );
}