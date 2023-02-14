namespace FictiveShop.Core.Domain
{
    public class Customer : BaseEntity
    {
        public string Name { get; init; }

        public string Address { get; set; }

        public string PhoneNumber { get; set; }
    }
}