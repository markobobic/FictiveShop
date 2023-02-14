using FictiveShop.Core.ValueObjects;

namespace FictiveShop.Core.Domain
{
    public class Customer : BaseEntity
    {
        public CustomerName Name { get; init; }
        public Address Address { get; set; }
        public string PhoneNumber { get; set; }
    }
}