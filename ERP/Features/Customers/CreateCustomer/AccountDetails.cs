namespace Catalog_Service.Features.Customers.CreateCustomer
{
    public class AccountDetails
    {
        public string? CustomerGroup { get; set; }
        public string? ReferralSource { get; set; }
        public string? RegisteredBy { get; set; }  // employee name
    }

}