namespace Catalog_Service.Features.Customers.CreateCustomer
{
    public class CreateCustomerRequest
    {
        public BasicInformation BasicInformation { get; set; }
        public AccountDetails AccountDetails { get; set; }
        public LocationAndCustom LocationAndCustom { get; set; }
    }
}