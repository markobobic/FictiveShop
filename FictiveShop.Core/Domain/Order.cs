using FictiveShop.Core.ValueObjects;

namespace FictiveShop.Core.Domain
{
    public class Order : BaseEntity
    {
        public decimal TotalAmount { get; set; }
        public decimal AppliedDiscount { get; set; }
        public string CustomerId { get; set; }
        public List<BasketItem> BasketItems { get; set; } = new();
        public Address ShippingAddress { get; set; }
        public DateTime OrderDate { get; set; } = DateTime.Now;
    }
}