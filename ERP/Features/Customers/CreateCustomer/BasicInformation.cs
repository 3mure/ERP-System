using System.ComponentModel.DataAnnotations;

namespace Catalog_Service.Features.Customers.CreateCustomer
{
    public class BasicInformation
    {
        [Required(ErrorMessage = "name_en is required.")]
        public string NameEn { get; set; }

        [Required(ErrorMessage = "phone_number is required.")]
        public string PhoneNumber { get; set; }

        [EmailAddress]
        public string? EmailAddress { get; set; }

        public string? Type { get; set; } = "Individual";

        [MinLength(8, ErrorMessage = "Password must be at least 8 characters.")]
        public string? Password { get; set; }
    }
}