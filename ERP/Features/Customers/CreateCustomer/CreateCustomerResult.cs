namespace Catalog_Service.Features.Customers.CreateCustomer
{
    public record CreateCustomerResult(
      bool Success,
      string Message,
      int? CustomerId = null
  );

}